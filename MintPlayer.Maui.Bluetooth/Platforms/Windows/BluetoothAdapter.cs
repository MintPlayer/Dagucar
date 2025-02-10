using MintPlayer.Maui.Bluetooth.EventArgs;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Enumeration;
using WindowsBluetoothAdapter = Windows.Devices.Bluetooth.BluetoothAdapter;
using WindowsBluetoothDevice = Windows.Devices.Bluetooth.BluetoothDevice;

namespace MintPlayer.Maui.Bluetooth;

public class BluetoothAdapter : IBluetoothAdapter
{
    private WindowsBluetoothAdapter? innerAdapter;
    private BluetoothLEAdvertisementWatcher watcher = new()
    {
        ScanningMode = BluetoothLEScanningMode.Passive,
    };

    public BluetoothAdapter()
    {
        watcher.Received += Watcher_Received;
        watcher.Stopped += Watcher_Stopped;
    }

    public static Task RequestPermissions() => Task.CompletedTask;

    public static async Task<BluetoothAdapter> GetDefaultAdapter()
    {
        var result = new BluetoothAdapter
        {
            innerAdapter = await WindowsBluetoothAdapter.GetDefaultAsync(),
        };
        return result;
    }

    public async Task GetPairedDevices()
    {
        //var result = await DeviceInformation.FindAllAsync(WindowsBluetoothDevice.GetDeviceSelectorFromPairingState(true));
        //result.ToArray().Select(x => x.)
        throw new NotImplementedException();
    }

    public Task<bool> StartDiscovery()
    {
        if (innerAdapter != null)
        {
            //var devices = await DeviceInformation.FindAllAsync();
            //if (DevicesDiscovered != null)
            //    DevicesDiscovered(this, new(devices.Select(d => new BluetoothDevice(d.)));

            watcher.Start();


            return Task.FromResult(true);
        }
        else
            return Task.FromResult(false);
    }

    public Task<bool> StopDiscovery()
    {
        if (watcher != null)
        {
            watcher.Stop();
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    private async void Watcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
    {
        if (DeviceDiscovered != null)
        {
            var device = await BluetoothLEDevice.FromBluetoothAddressAsync(args.BluetoothAddress);
            DeviceDiscovered(this, new(new BluetoothDevice(device)));
        }
    }

    private void Watcher_Stopped(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
    {
        watcher.Received -= Watcher_Received;
        watcher.Stopped -= Watcher_Stopped;
    }


    public event EventHandler<DevicesDiscoveredEventArgs>? DeviceDiscovered;
}
