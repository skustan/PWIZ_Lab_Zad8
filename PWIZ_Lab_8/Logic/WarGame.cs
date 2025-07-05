using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using PWIZ_Lab_8.Models;

namespace PWIZ_Lab_8.Logic
{
    public class WarGame
    {
        private Queue<Card> _playerDeck = new();
        private Queue<Card> _computerDeck = new();
        private bool _gameOver = false;
        private Random _random = new();

        public Queue<Card> PlayerDeck => _playerDeck;
        public Queue<Card> ComputerDeck => _computerDeck;

        public bool IsGameOver => _gameOver || _playerDeck.Count == 0 || _computerDeck.Count == 0;

        public WarGame()
        {
            var fullDeck = new Deck();
            var shuffledCards = new Queue<Card>();

            while (fullDeck.Count > 0)
            {
                var card = fullDeck.Draw();
                if (card != null)
                    shuffledCards.Enqueue(card);
            }

            bool toPlayer = true;
            while (shuffledCards.Count > 0)
            {
                if (toPlayer)
                    _playerDeck.Enqueue(shuffledCards.Dequeue());
                else
                    _computerDeck.Enqueue(shuffledCards.Dequeue());

                toPlayer = !toPlayer;
            }
        }

        private (List<Card> warPile, string result, bool warEnded) ResolveWar(List<Card> warPile)
        {
            const int warCardsCount = 3;

            if (_playerDeck.Count < warCardsCount + 1)
            {
                EndGame();
                return (warPile, "Gracz nie ma wystarczająco kart na wojnę. Komputer wygrywa grę!", true);
            }
            if (_computerDeck.Count < warCardsCount + 1)
            {
                EndGame();
                return (warPile, "Komputer nie ma wystarczająco kart na wojnę. Gracz wygrywa grę!", true);
            }

            for (int i = 0; i < warCardsCount; i++)
            {
                warPile.Add(_playerDeck.Dequeue());
                warPile.Add(_computerDeck.Dequeue());
            }

            var playerCard = _playerDeck.Dequeue();
            var computerCard = _computerDeck.Dequeue();

            warPile.Add(playerCard);
            warPile.Add(computerCard);

            string result;

            if (playerCard.Value > computerCard.Value)
            {
                foreach (var c in warPile)
                    _playerDeck.Enqueue(c);

                result = $"Wojna zakończona! Gracz wygrywa wojenną rundę kartą {playerCard.Value}";
                return (warPile, result, true);
            }
            else if (playerCard.Value < computerCard.Value)
            {
                foreach (var c in warPile)
                    _computerDeck.Enqueue(c);

                result = $"Wojna zakończona! Komputer wygrywa wojenną rundę kartą {computerCard.Value}";
                return (warPile, result, true);
            }
            else
            {
                result = "Kolejny remis podczas wojny! Wojna trwa dalej...";
                return (warPile, result, false);
            }
        }

        public (Card? playerCard, Card? computerCard, string result) PlayRound()
        {
            if (_playerDeck.Count == 0 || _computerDeck.Count == 0)
            {
                EndGame();
                return (null, null, "Gra zakończona!");
            }

            var playerCard = _playerDeck.Dequeue();
            var computerCard = _computerDeck.Dequeue();

            var warPile = new List<Card> { playerCard, computerCard };

            string result;

            if (playerCard.Value > computerCard.Value)
            {
                _playerDeck.Enqueue(playerCard);
                _playerDeck.Enqueue(computerCard);
                result = "Gracz wygrywa rundę!";
            }
            else if (playerCard.Value < computerCard.Value)
            {
                _computerDeck.Enqueue(playerCard);
                _computerDeck.Enqueue(computerCard);
                result = "Komputer wygrywa rundę!";
            }
            else
            {
                bool warEnded = false;
                result = "Remis! Rozpoczyna się wojna...";

                while (!warEnded)
                {
                    var warResult = ResolveWar(warPile);
                    warPile = warResult.warPile;
                    result = warResult.result;
                    warEnded = warResult.warEnded;

                    if (warEnded)
                        break;

                    if (_playerDeck.Count == 0 || _computerDeck.Count == 0)
                    {
                        EndGame();
                        result = "Gra zakończona podczas wojny!";
                        warEnded = true;
                        break;
                    }
                }
            }

            _playerDeck = ShuffleQueue(_playerDeck);
            _computerDeck = ShuffleQueue(_computerDeck);

            return (playerCard, computerCard, result);
        }

        public void EndGame()
        {
            _gameOver = true;

            var result = new WarGameResult
            {
                Winner = GetWinner(),
                PlayerCardsLeft = _playerDeck.Count,
                ComputerCardsLeft = _computerDeck.Count,
                GameEndTime = DateTime.Now
            };

            SaveResult(GetWinner());

        }

        public string GetWinner()
        {
            if (_playerDeck.Count > _computerDeck.Count)
                return "Wygrama";
            else if (_playerDeck.Count < _computerDeck.Count)
                return "Przegrana";
            else
                return "Remis";
        }

        private Queue<T> ShuffleQueue<T>(Queue<T> inputQueue)
        {
            var list = new List<T>(inputQueue);
            int n = list.Count;

            for (int i = n - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }

            return new Queue<T>(list);
        }


        private void SaveResult(string result)
        {
            var gameResult = new WarGameResult
            {
                Winner = result,
                PlayerCardsLeft = _playerDeck.Count,
                ComputerCardsLeft = _computerDeck.Count,
                GameEndTime = DateTime.Now
            };

            string baseDir = AppContext.BaseDirectory;
            string projectRoot = Path.GetFullPath(Path.Combine(baseDir, @"..\..\.."));
            string folderPath = Path.Combine(projectRoot, "Results");
            string path = Path.Combine(folderPath, "WarGameResults.json");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            List<WarGameResult> results = new();

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    results = JsonSerializer.Deserialize<List<WarGameResult>>(json) ?? new List<WarGameResult>();
                }
            }

            results.Add(gameResult);

            string newJson = JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, newJson);
        }

    }
}
