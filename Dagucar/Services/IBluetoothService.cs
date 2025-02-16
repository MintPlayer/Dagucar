using System.Collections.ObjectModel;

namespace Dagucar.Services;

public interface IBluetoothService : IDisposable
{
    Task<bool> StartDiscovery(Func<string, Task> deviceFound, Func<Task> discoveryFinished);
    Task<bool> StopDiscovery();
    //bool IsDiscovering { get; }
}
