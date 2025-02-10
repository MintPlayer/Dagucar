namespace MintPlayer.Maui.Bluetooth.EventArgs;

public class DevicesDiscoveredEventArgs : System.EventArgs
{
    public DevicesDiscoveredEventArgs(IBluetoothDevice device)
    {
        Device = device;
    }

    public IBluetoothDevice Device { get; private set; }
}
