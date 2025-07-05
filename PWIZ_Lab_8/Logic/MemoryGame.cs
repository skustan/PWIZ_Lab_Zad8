using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PWIZ_Lab_8.Models;

namespace PWIZ_Lab_8.Logic
{
    public class MemoryGame
    {
        private readonly List<Card> _boardCards;
        private readonly bool[] _matched;
        private int? _firstSelectedIndex = null;
        private int _moves = 0;
        private DateTime _startTime;
        private bool _isChecking = false;

        public event Action<int> MoveCountChanged;
        public event Action<TimeSpan> TimeChanged;
        public event Action<int, Card> CardRevealed;
        public event Action<int> CardHidden;
        public event Action GameWon;

        public IReadOnlyList<Card> BoardCards => _boardCards;
        public IReadOnlyList<bool> Matched => _matched;
        public int Moves => _moves;

        public MemoryGame()
        {
            _boardCards = new List<Card>();
            _matched = new bool[16];
            InitializeBoard();
            _startTime = DateTime.Now;
            StartTimer();
        }

        private void InitializeBoard()
        {
            var deck = new Deck();

            var uniqueCards = deck.Cards.OrderBy(_ => Guid.NewGuid()).Take(8).ToList();


            var pairs = uniqueCards.Concat(uniqueCards).OrderBy(_ => Guid.NewGuid()).ToList();

            _boardCards.Clear();
            _boardCards.AddRange(pairs);

            for (int i = 0; i < _matched.Length; i++)
                _matched[i] = false;
        }

        private async void StartTimer()
        {
            while (!_matched.All(m => m))
            {
                TimeChanged?.Invoke(DateTime.Now - _startTime);
                await Task.Delay(1000);
            }
        }


        public async Task<bool> SelectCardAsync(int index)
        {
            if (_isChecking || index < 0 || index >= _boardCards.Count || _matched[index])
                return false;

            CardRevealed?.Invoke(index, _boardCards[index]);

            if (_firstSelectedIndex == null)
            {
                _firstSelectedIndex = index;
                return true;
            }
            else
            {
                _moves++;
                MoveCountChanged?.Invoke(_moves);

                int first = _firstSelectedIndex.Value;
                int second = index;

                if (AreCardsMatching(_boardCards[first], _boardCards[second]))
                {
                    _matched[first] = true;
                    _matched[second] = true;

                    if (_matched.All(m => m))
                        GameWon?.Invoke();

                    _firstSelectedIndex = null;
                    return true;
                }
                else
                {
                    _isChecking = true;

                    await Task.Delay(750);

                    CardHidden?.Invoke(first);
                    CardHidden?.Invoke(second);

                    _firstSelectedIndex = null;
                    _isChecking = false;
                    return true;
                }
            }
        }

        private bool AreCardsMatching(Card c1, Card c2)
        {
            return c1.Suit == c2.Suit && c1.Value == c2.Value;
        }
    }
}
