using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using PWIZ_Lab_8.Logic;
using PWIZ_Lab_8.Models;
using static PWIZ_Lab_8.UserControlBlackJack;


namespace PWIZ_Lab_8.Views
{
    public partial class UserControlMemory : UserControl
    {
        private readonly MainWindow _mainWindow;

        private MemoryGame _game;
        private List<Button> _cardButtons;

        const int cardWidth = 100;
        const int cardHeight = 100;

        MemoryGameResult memoryGameResult = new();


        public UserControlMemory(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;


            _game = new MemoryGame();
            _game.MoveCountChanged += OnMoveCountChanged;
            _game.TimeChanged += OnTimeChanged;
            _game.CardRevealed += OnCardRevealed;
            _game.CardHidden += OnCardHidden;
            _game.GameWon += OnGameWon;


            InitializeBoardUI();

        }

        public UserControlMemory()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            BoardGrid = this.FindControl<UniformGrid>("BoardGrid");
            RestartButton = this.FindControl<Button>("RestartButton");
            MovesText = this.FindControl<TextBlock>("MovesText");
            TimeText = this.FindControl<TextBlock>("TimeText");
            WinText = this.FindControl<TextBlock>("WinText");
            BackToMenuButton = this.FindControl<Button>("BackToMenuButton");

            RestartButton.Click += RestartGame;
            BackToMenuButton.Click += ToMenu;
        }

        private void ToMenu(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateTo(new MainMenu(_mainWindow));
        }

        public Image CardBack(int width = cardWidth, int height = cardHeight)
        {
            var uri = new Uri($"avares://PWIZ_Lab_8/Assets/Cards/Card Back.png");
            var img = new Image
            {
                Source = new Bitmap(AssetLoader.Open(uri)),
                Width = width,
                Height = height
            };
            return img;
        }

        public Image CardFront(Card card ,int width = cardWidth, int height = cardHeight)
        {
            var uri = new Uri($"avares://PWIZ_Lab_8{card.ImagePath}");
            var img = new Image
            {
                Source = new Bitmap(AssetLoader.Open(uri)),
                Width = width,
                Height = height
            };
            return img;
        }

        private void InitializeBoardUI()
        {
            _cardButtons = new List<Button>();

            BoardGrid.Children.Clear();

            for (int i = 0; i < 16; i++)
            {
                var btn = new Button
                {
                    Tag = i,
                    Content = CardBack(),
                    MinWidth = cardWidth,
                    MinHeight = cardHeight,
                };



                btn.Click += CardButton_Click;
                _cardButtons.Add(btn);
                BoardGrid.Children.Add(btn);
            }
        }

        private async void CardButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int index)
            {
                bool canSelect = await _game.SelectCardAsync(index);
            }
        }

        private void OnMoveCountChanged(int moves)
        {
            MovesText.Text = moves.ToString();
        }
        
        private void OnTimeChanged(TimeSpan time)
        {
            TimeText.Text = time.ToString(@"mm\:ss");
        }

        private void OnCardRevealed(int index, Card card)
        {
            var btn = _cardButtons[index];
            btn.Content = CardFront(card);
            btn.IsEnabled = false;
        }

        private void OnCardHidden(int index)
        {
            var btn = _cardButtons[index];
            btn.Content = CardBack();
            btn.IsEnabled = true;
        }

        private void OnGameWon()
        {
            WinText.Text = "Gratulacje! Wygrałeś!";

            memoryGameResult.Time = DateTime.Now;
            memoryGameResult.GameTime = TimeText.Text;
            memoryGameResult.Moves = Convert.ToInt32(MovesText.Text);


            string baseDir = AppContext.BaseDirectory;
            string projectRoot = Path.GetFullPath(Path.Combine(baseDir, @"..\..\.."));
            string folderPath = Path.Combine(projectRoot, "Results");
            string path = Path.Combine(folderPath, "MemoryGameResults.json");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            List<MemoryGameResult> results = new();

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    results = JsonSerializer.Deserialize<List<MemoryGameResult>>(json) ?? new List<MemoryGameResult>();
                }
            }

            results.Add(memoryGameResult);

            string newJson = JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, newJson);

        }


        private void RestartGame(object sender, RoutedEventArgs e)
        {
            _game.MoveCountChanged -= OnMoveCountChanged;
            _game.TimeChanged -= OnTimeChanged;
            _game.CardRevealed -= OnCardRevealed;
            _game.CardHidden -= OnCardHidden;
            _game.GameWon -= OnGameWon;

            _game = new MemoryGame();

            _game.MoveCountChanged += OnMoveCountChanged;
            _game.TimeChanged += OnTimeChanged;
            _game.CardRevealed += OnCardRevealed;
            _game.CardHidden += OnCardHidden;
            _game.GameWon += OnGameWon;

            WinText.Text = "";
            MovesText.Text = "0";
            TimeText.Text = "00:00";

            for (int i = 0; i < _cardButtons.Count; i++)
            {
                _cardButtons[i].Content = CardBack();
                _cardButtons[i].IsEnabled = true;
            }
        }


    }
}
