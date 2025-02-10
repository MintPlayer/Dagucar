using MintPlayer.Maui.Bluetooth.EventArgs;
using AndroidBluetoothAdapter = Android.Bluetooth.BluetoothAdapter;
using AndroidBluetoothDevice = Android.Bluetooth.BluetoothDevice;

namespace MintPlayer.Maui.Bluetooth;

public class BluetoothAdapter : IBluetoothAdapter
{
    private AndroidBluetoothAdapter? innerAdapter;

    public static Task RequestPermissions()
    {
        return Task.CompletedTask;
    }

    public static Task<BluetoothAdapter> GetDefaultAdapter()
    {
        var result = new BluetoothAdapter
        {
            innerAdapter = AndroidBluetoothAdapter.DefaultAdapter
        };
        return Task.FromResult(result);
    }

    public Task<bool> StartDiscovery()
    {
        if (innerAdapter != null)
        {
            var result = innerAdapter.StartDiscovery();
            return Task.FromResult(result);
        }
        else
            return Task.FromResult(false);
    }

    public Task<bool> StopDiscovery()
    {
        if (innerAdapter != null)
        {
            var result = innerAdapter.CancelDiscovery();
            return Task.FromResult(result);
        }
        else
            return Task.FromResult(false);
    }

    public event EventHandler<DevicesDiscoveredEventArgs>? DeviceDiscovered;
}
