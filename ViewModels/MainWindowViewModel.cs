using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DDNScover.Models;
using DDNScover.Services;

namespace DDNScover.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly DnsService _dnsService;
    private readonly PingService _pingService;

    [ObservableProperty]
    private string _searchQuery = "";

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _statusMessage = "Ready to search.";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasResults))]
    private ObservableCollection<DiscoveredHost> _searchResults = new();

    public bool HasResults => SearchResults.Count > 0;

    public MainWindowViewModel()
    {
        _dnsService = new DnsService();
        _pingService = new PingService();
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            StatusMessage = "Please enter a domain.";
            return;
        }

        IsBusy = true;
        StatusMessage = $"Searching for subdomains of {SearchQuery}...";
        SearchResults.Clear();

        try
        {
            var results = await _dnsService.GetSubdomainsAsync(SearchQuery);
            foreach (var result in results)
            {
                SearchResults.Add(result);
                _ = QueuePing(result);
            }
            StatusMessage = $"Found {results.Count} subdomains.";
            OnPropertyChanged(nameof(HasResults));
        }
        catch (System.Exception ex)
        {
             StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task QueuePing(DiscoveredHost host)
    {
        await Task.Run(async () => 
        {
            var isUp = await _pingService.PingHostAsync(host.Hostname);
            Avalonia.Threading.Dispatcher.UIThread.Post(() => 
            {
                host.IsUp = isUp;
                host.Status = isUp ? "Up" : "Down/Unreachable";
            });
        });
    }

    [RelayCommand]
    private async Task ExportAsync()
    {
        if (SearchResults.Count == 0)
        {
            StatusMessage = "No results to export.";
            return;
        }

        try
        {
            var desktop = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            var filename = $"DDNScover_{SearchQuery}_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv";
            var path = System.IO.Path.Combine(desktop, filename);

            var lines = new System.Collections.Generic.List<string>();
            lines.Add("Hostname,IP Status,Source");
            
            foreach (var item in SearchResults)
            {
                var status = item.IsUp == true ? "Up" : "Down/Unreachable";
                lines.Add($"{item.Hostname},{status},\"{item.Source}\"");
            }

            await System.IO.File.WriteAllLinesAsync(path, lines);
            StatusMessage = $"Exported to Desktop: {filename}";
        }
        catch (System.Exception ex)
        {
            StatusMessage = $"Export Error: {ex.Message}";
        }
    }
}
