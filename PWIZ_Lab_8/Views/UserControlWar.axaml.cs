using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using PWIZ_Lab_8.Models;
using PWIZ_Lab_8.Views;
using PWIZ_Lab_8.Logic;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Diagnostics;

namespace PWIZ_Lab_8;

public partial class UserControlWar : UserControl
{
    private readonly MainWindow _mainWindow;
    private WarGame _game;


    public UserControlWar() => InitializeComponent();

    public UserControlWar(MainWindow mainWindow)
    {
        InitializeComponent();
        _mainWindow = mainWindow;
        _game = new WarGame();

        UpdateCardCounts();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnBackToMenu(object? sender, RoutedEventArgs e)
    {
        _mainWindow?.NavigateTo(new MainMenu(_mainWindow));
    }

    private void OnPlayRound(object? sender, RoutedEventArgs e)
    {
        (Card? playerCard, Card? computerCard, string result) = _game.PlayRound();

        this.FindControl<TextBlock>("RoundResultText").Text = result;

        var playerPanel = this.FindControl<StackPanel>("PlayerPanel");
        var computerPanel = this.FindControl<StackPanel>("ComputerPanel");

        playerPanel.Children.Clear();
        computerPanel.Children.Clear();

        AddCardToPanel(playerCard, playerPanel);
        AddCardToPanel(computerCard, computerPanel);

        UpdateCardCounts();

        if (_game.IsGameOver)
        {
            this.FindControl<Button>("PlayRoundButton").IsEnabled = false;

            this.FindControl<TextBlock>("RoundResultText").Text +=
                $"\n{_game.GetWinner()}";
        }

    }

    private void UpdateCardCounts()
    {
        this.FindControl<TextBlock>("PlayerCountText").Text = $"Karty gracza: {_game.PlayerDeck.Count}";
        this.FindControl<TextBlock>("ComputerCountText").Text = $"Karty komputera: {_game.ComputerDeck.Count}";
    }

    private void AddCardToPanel(Card card, Panel panel)
    {
        Debug.WriteLine($"Loading image from path: {card.ImagePath}");


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


}