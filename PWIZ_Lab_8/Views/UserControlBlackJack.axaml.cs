using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PWIZ_Lab_8.Views;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Interactivity;
using System;
using PWIZ_Lab_8.Models;
using PWIZ_Lab_8.Logic;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Diagnostics;

namespace PWIZ_Lab_8;

public partial class UserControlBlackJack : UserControl
{
    private readonly MainWindow _mainWindow;

    private OczkoGame _game = new();

    public class GameResult
    {
        public string Result { get; set; }
        public int PlayerPoints { get; set; }
        public int DealerPoints { get; set; }
        public DateTime Time { get; set; }
    }

    public UserControlBlackJack() { InitializeComponent(); }

    public UserControlBlackJack(MainWindow mainWindow)
    {
        InitializeComponent();
        _mainWindow = mainWindow;

    }

    private void OnBackToMenu(object? sender, RoutedEventArgs e)
    {
        _mainWindow.NavigateTo(new MainMenu(_mainWindow));
    }

    private void OnHit(object? sender, RoutedEventArgs e)
    {
        var card = _game.PlayerDraw();
        if (card != null)
        {
            AddCard(card, PlayerPanel);
            int points = _game.CalculatePoints(_game.PlayerHand);
            PlayerPointsText.Text = $"Punkty: {points}";

            if (points > 21)
            {
                EndGame(_game.CheckWinner());
            }
            else if (points == 21)
            {
                OnStand(null, null);
            }
        }

    }

    private void OnStand(object? sender, RoutedEventArgs e)
    {
        _game.DealerPlay();

        DealerPanel.Children.Clear();
        foreach (var card in _game.DealerHand)
            AddCard(card, DealerPanel);

        DealerPointsText.Text = $"Punkty: {_game.CalculatePoints(_game.DealerHand)}";
        EndGame(_game.CheckWinner());
    }

    private void OnRestart(object? sender, RoutedEventArgs e)
    {
        _game.Restart();

        HitButton.IsEnabled = true;
        StandButton.IsEnabled = true;

        PlayerPanel.Children.Clear();
        DealerPanel.Children.Clear();
        PlayerPointsText.Text = "Punkty: 0";
        DealerPointsText.Text = "Punkty: 0";
        ResultText.Text = "";
    }

    private void AddCard(Card card, Panel panel)
    {
        var uri = new Uri($"avares://PWIZ_Lab_8{card.ImagePath}");
        var bitmap = new Bitmap(AssetLoader.Open(uri));

        var image = new Image
        {
            Source = bitmap,
            Width = 80,
            Height = 120,
            Margin = new Thickness(4)
        };

        panel.Children.Add(image);
    }

    private void EndGame(string result)
    {
        ResultText.Text = result;
        HitButton.IsEnabled = false;
        StandButton.IsEnabled = false;

        try
        {
            SaveResult(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd zapisu: {ex.Message}");
        }

    }


    private void SaveResult(string result)
    {
        var gameResult = new GameResult
        {
            Result = result,
            PlayerPoints = _game.CalculatePoints(_game.PlayerHand),
            DealerPoints = _game.CalculatePoints(_game.DealerHand),
            Time = DateTime.Now
        };

        string baseDir = AppContext.BaseDirectory;
        string projectRoot = Path.GetFullPath(Path.Combine(baseDir, @"..\..\.."));
        string folderPath = Path.Combine(projectRoot, "Results");
        string path = Path.Combine(folderPath, "OczkoGameResults.json");

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        List<GameResult> results = new();

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            results = JsonSerializer.Deserialize<List<GameResult>>(json) ?? new List<GameResult>();
        }

        results.Add(gameResult);

        string newJson = JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, newJson);

    }
}