using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PWIZ_Lab_8.Models;

namespace PWIZ_Lab_8;

public partial class UserControlScoreBoard : UserControl
{

    public UserControlScoreBoard()
    {
        InitializeComponent();

        LoadOczkoResults();
        LoadWarResults();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        OczkoResultsListBox = this.FindControl<ListBox>("OczkoResultsListBox");
        WarResultsListBox = this.FindControl<ListBox>("WarResultsListBox");
    }

    private void LoadOczkoResults()
    {
        string baseDir = AppContext.BaseDirectory;
        string projectRoot = Path.GetFullPath(Path.Combine(baseDir, @"..\..\.."));
        string path = Path.Combine(projectRoot, "Results", "OczkoGameResults.json");

        if (!File.Exists(path))
        {
            OczkoResultsListBox.ItemsSource = new List<OczkoGameResult>();
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            var results = JsonSerializer.Deserialize<List<OczkoGameResult>>(json) ?? new List<OczkoGameResult>();
            var sorted = results.OrderByDescending(r => r.Time).ToList();
            OczkoResultsListBox.ItemsSource = sorted;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas ładowania wyników Oczko: {ex.Message}");
        }
    }

    private void LoadWarResults()
    {
        string baseDir = AppContext.BaseDirectory;
        string projectRoot = Path.GetFullPath(Path.Combine(baseDir, @"..\..\.."));
        string path = Path.Combine(projectRoot, "Results", "WarGameResults.json");

        if (!File.Exists(path))
        {
            WarResultsListBox.ItemsSource = new List<WarGameResult>();
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            var results = JsonSerializer.Deserialize<List<WarGameResult>>(json) ?? new List<WarGameResult>();
            var sorted = results.OrderByDescending(r => r.GameEndTime).ToList();
            WarResultsListBox.ItemsSource = sorted;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas ładowania wyników Wojny: {ex.Message}");
        }
    }
}
