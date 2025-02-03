using System.Collections.ObjectModel;

namespace Dagucar.Services;

public interface IBluetoothService : IDisposable
{
    void StartDiscovery(Action<string> deviceFound, Action discoveryFinished);
    bool IsDiscovering { get; }
}
