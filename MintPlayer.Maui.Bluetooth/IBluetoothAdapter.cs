namespace MintPlayer.Maui.Bluetooth;

public interface IBluetoothAdapter
{
    Task<bool> StartDiscovery();
    Task<bool> StopDiscovery();
    event EventHandler<EventArgs.DevicesDiscoveredEventArgs>? DeviceDiscovered;
}
