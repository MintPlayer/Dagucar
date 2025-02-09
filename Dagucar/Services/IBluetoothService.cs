using Dagucar.Services.EventArgs;
using System.Collections.ObjectModel;

namespace Dagucar.Services;

public interface IBluetoothService : IDisposable
{
    Task RequestBluetoothPermissions();
    event EventHandler GotPermissions;
    void OnGotPermissions();
    Task<IEnumerable<BluetoothDevice>> GetBondedDevices();
    Task StartDiscovery();
    event EventHandler DiscoveryStarted;
    event EventHandler<DeviceDiscoveredEventArgs> DeviceFound;
    Task StopDiscovery();
    event EventHandler DiscoveryFinished;
    Task CreateDeviceBond(BluetoothDevice bluetoothDevice);
    event EventHandler<BondStateChangedEventArgs> BondStateChanged;
    Task ConnectToDevice(BluetoothDevice bluetoothDevice);
}
