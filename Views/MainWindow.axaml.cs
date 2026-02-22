using Avalonia.Controls;

namespace DDNScover.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void CopyHostname_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is MenuItem menuItem && 
            menuItem.DataContext is Models.DiscoveredHost host)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel?.Clipboard is { } clipboard)
            {
                await clipboard.SetTextAsync(host.Hostname);
                
                if (DataContext is ViewModels.MainWindowViewModel vm)
                {
                   vm.StatusMessage = $"Copied: {host.Hostname}";
                }
            }
        }
    }
}