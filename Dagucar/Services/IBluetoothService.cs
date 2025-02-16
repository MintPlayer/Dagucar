using System.Collections.ObjectModel;

namespace Dagucar.Services;

public interface IBluetoothService : IDisposable
{
    Task<bool> StartDiscovery(Func<BluetoothDevice, Task> deviceFound, Func<Task> discoveryFinished);
    Task<bool> StopDiscovery();
    Task<IEnumerable<BluetoothDevice>> GetBondedDevices();
    Task<bool> CreateBond(BluetoothDevice device);
    //bool IsDiscovering { get; }
}

public class BluetoothDevice
{
    public string Name { get; set; }
    public string Address { get; set; }
}