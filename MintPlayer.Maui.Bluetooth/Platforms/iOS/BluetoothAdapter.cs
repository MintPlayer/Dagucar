using MintPlayer.Maui.Bluetooth.EventArgs;

namespace MintPlayer.Maui.Bluetooth;

public class BluetoothAdapter : IBluetoothAdapter
{

    public static Task RequestPermissions() => throw new NotImplementedException();

    public static Task<BluetoothAdapter> GetDefaultAdapter() => throw new NotImplementedException();
    public Task<bool> StartDiscovery() => throw new NotImplementedException();
    public Task<bool> StopDiscovery() => throw new NotImplementedException();
    public event EventHandler<DevicesDiscoveredEventArgs>? DeviceDiscovered;
}
