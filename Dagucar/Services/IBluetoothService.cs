using System.Collections.ObjectModel;

namespace Dagucar.Services;

public interface IBluetoothService : IDisposable
{
    Task StartDiscovery();
    bool IsDiscovering { get; }
    ObservableCollection<string> BluetoothDevices { get; }
}
