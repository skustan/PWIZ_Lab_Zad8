using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PWIZ_Lab_8.Models; // używaj modelu OczkoGameResult z osobnego pliku

namespace PWIZ_Lab_8;

public partial class UserControlScoreBoard : UserControl
{
    public UserControlScoreBoard()
    {
        InitializeComponent();
        LoadOczkoResults(); // dopiero po przypisaniu ResultsListBox
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        ResultsListBox = this.FindControl<ListBox>("ResultsListBox");
    }

    private void LoadOczkoResults()
    {
        string baseDir = AppContext.BaseDirectory;
        string projectRoot = Path.GetFullPath(Path.Combine(baseDir, @"..\..\.."));
        string path = Path.Combine(projectRoot, "Results", "OczkoGameResults.json");

        if (!File.Exists(path))
        {
            ResultsListBox.ItemsSource = new List<OczkoGameResult>();
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            var results = JsonSerializer.Deserialize<List<OczkoGameResult>>(json) ?? new List<OczkoGameResult>();
            var sorted = results.OrderByDescending(r => r.Time).ToList();
            ResultsListBox.ItemsSource = sorted;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas ładowania wyników: {ex.Message}");
        }
    }
}
