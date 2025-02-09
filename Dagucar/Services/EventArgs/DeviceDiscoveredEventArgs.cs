namespace Dagucar.Services.EventArgs;

public class DeviceDiscoveredEventArgs : System.EventArgs
{
    public required BluetoothDevice Device { get; init; }
}
