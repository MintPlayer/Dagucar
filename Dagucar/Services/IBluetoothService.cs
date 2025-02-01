using System.Collections.ObjectModel;

namespace Dagucar.Services;

public interface IBluetoothService : IDisposable
{
    Task StartDiscovery();
    ObservableCollection<string> BluetoothDevices { get; }
}
