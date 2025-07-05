using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;


namespace PWIZ_Lab_8.Views;

public partial class MainMenu : UserControl
{
    private readonly MainWindow _mainWindow;

    public MainMenu() { InitializeComponent(); }

    public MainMenu(MainWindow mainWindow)
    {
        InitializeComponent();
        _mainWindow = mainWindow;

        ButtonScoreBoard.Click += ButtonScoreBoard_OnClick;
        ButtonExit.Click += ButtonExit_OnClick;
        ButtonPlayBlackJack.Click += ButtonPlayBlackJack_OnClick;
        ButtonPlayWar.Click += ButtonPlayWar_OnClick;
    }

    private void ButtonPlayBlackJack_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _mainWindow.NavigateTo(new UserControlBlackJack(_mainWindow));
    }

    private void ButtonPlayWar_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _mainWindow.NavigateTo(new UserControlWar(_mainWindow));
    }

    private void ButtonScoreBoard_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        WindowScoreBoard windowScoreBoard = new();
        windowScoreBoard.Show();
    }

    private void ButtonExit_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }

}
