using System.Collections.ObjectModel;

namespace Dagucar.Services;

public interface IBluetoothService : IDisposable
{
    Task StartDiscovery(Func<string, Task> deviceFound, Func<Task> discoveryFinished);
    bool IsDiscovering { get; }
}
