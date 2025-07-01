using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;


namespace PWIZ_Lab_8.Views;

public partial class MainMenu : UserControl
{
    public MainMenu()
    {
        InitializeComponent();

        ButtonScoreBoard.Click += ButtonScoreBoard_OnClick;
        ButtonExit.Click += ButtonExit_OnClick;
    }

    private void ButtonScoreBoard_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        WindowScoreBoard windowScoreBoard = new WindowScoreBoard();
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
