using CommunityToolkit.Mvvm.ComponentModel;

namespace DDNScover.Models;

public partial class DiscoveredHost : ObservableObject
{
    public required string Hostname { get; set; }
    public string Source { get; set; } = "crt.sh";

    [ObservableProperty]
    private string _status = "Checking...";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusColor))]
    private bool? _isUp;

    public string StatusColor => IsUp switch
    {
        true => "#4CC9B0", 
        false => "#F14C4C", 
        null => "#808080"   
    };
}
