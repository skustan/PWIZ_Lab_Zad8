using Avalonia.Controls;

namespace PWIZ_Lab_8.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        MainContent.Content = new MainMenu(this);
    }

    public void NavigateTo(UserControl control)
    {
        MainContent.Content = control;
    }

}
