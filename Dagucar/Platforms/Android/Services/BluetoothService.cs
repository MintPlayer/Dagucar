using Android.Bluetooth;
using AndroidX.Core.App;
using Dagucar.Platforms.Android.CustomCode;
using Dagucar.Services;
using Dagucar.Services.EventArgs;
using Java.Util;
using AndroidBluetoothDevice = Android.Bluetooth.BluetoothDevice;
using BluetoothDevice = Dagucar.Services.BluetoothDevice;

namespace Dagucar.Platforms.Android.Services;

internal class BluetoothService : IBluetoothService
{
    private readonly BluetoothAdapter? bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
    private readonly global::Android.Content.Context context;
    CustomCode.BluetoothReceiver? bluetoothReceiver = new();
    private readonly Dictionary<BluetoothDevice, AndroidBluetoothDevice> foundDevices = new();
    private readonly Dictionary<BluetoothDevice, AndroidBluetoothDevice> bondedDevices = new();

    public BluetoothService()
    {
        context = global::Microsoft.Maui.ApplicationModel.Platform.CurrentActivity
            ?? global::Microsoft.Maui.MauiApplication.Context;

        if (bluetoothAdapter == null || !bluetoothAdapter.IsEnabled)
            throw new Exception("Bluetooth not available/enabled");

        bluetoothReceiver = new BluetoothReceiver();

        bluetoothReceiver.DiscoveryStarted += BluetoothReceiver_DiscoveryStarted;
        bluetoothReceiver.DiscoveryFinished += BluetoothReceiver_DiscoveryFinished;
        bluetoothReceiver.DeviceFound += BluetoothReceiver_DeviceFound;
        bluetoothReceiver.UuidFetched += BluetoothReceiver_UuidFetched;
        bluetoothReceiver.BondStateChanged += BluetoothReceiver_BondStateChanged;

        foreach (var action in new[] { AndroidBluetoothDevice.ActionFound, BluetoothAdapter.ActionDiscoveryStarted, BluetoothAdapter.ActionDiscoveryFinished, AndroidBluetoothDevice.ActionBondStateChanged })
            context.RegisterReceiver(bluetoothReceiver, new global::Android.Content.IntentFilter(action));
    }

    public void Dispose()
    {
        if (bluetoothReceiver != null)
        {
            foreach (var action in new[] { AndroidBluetoothDevice.ActionFound, BluetoothAdapter.ActionDiscoveryStarted, BluetoothAdapter.ActionDiscoveryFinished, AndroidBluetoothDevice.ActionBondStateChanged })
                context.UnregisterReceiver(bluetoothReceiver);

            bluetoothReceiver.DiscoveryStarted -= BluetoothReceiver_DiscoveryStarted;
            bluetoothReceiver.DiscoveryFinished -= BluetoothReceiver_DiscoveryFinished;
            bluetoothReceiver.DeviceFound -= BluetoothReceiver_DeviceFound;
            bluetoothReceiver.UuidFetched -= BluetoothReceiver_UuidFetched;

            bluetoothReceiver.Dispose();
            bluetoothReceiver = null;
        }
    }

    public Task RequestBluetoothPermissions()
    {
        ActivityCompat.RequestPermissions(global::Microsoft.Maui.ApplicationModel.Platform.CurrentActivity!, [
            global::Android.Manifest.Permission.Bluetooth,
                global::Android.Manifest.Permission.BluetoothAdmin,
                global::Android.Manifest.Permission.BluetoothAdvertise,
                global::Android.Manifest.Permission.BluetoothConnect,
                global::Android.Manifest.Permission.BluetoothPrivileged,
                global::Android.Manifest.Permission.BluetoothScan,
                global::Android.Manifest.Permission.AccessCoarseLocation,
                global::Android.Manifest.Permission.AccessFineLocation,
                //"android.hardware.sensor.accelerometer"
            ], 1);

        return Task.CompletedTask;
    }

    public event EventHandler GotPermissions;
    public event EventHandler DiscoveryStarted;
    public event EventHandler<DeviceDiscoveredEventArgs> DeviceFound;
    public event EventHandler DiscoveryFinished;
    public event EventHandler<BondStateChangedEventArgs> BondStateChanged;


    public void OnGotPermissions()
    {
        if (GotPermissions != null) GotPermissions(this, EventArgs.Empty);
    }

    public Task StartDiscovery()
    {
        foundDevices.Clear();
        BluetoothAdapter.DefaultAdapter!.StartDiscovery();
        return Task.CompletedTask;
    }

    private void BluetoothReceiver_UuidFetched(object? sender, Platforms.Android.CustomCode.EventArgs.UuidFetchedEventArgs e)
    {
    }

    private void BluetoothReceiver_DiscoveryStarted(object? sender, EventArgs e)
    {
        if (DiscoveryStarted != null)
            DiscoveryStarted(this, EventArgs.Empty);
    }

    private void BluetoothReceiver_DeviceFound(object? sender, Platforms.Android.CustomCode.EventArgs.DeviceFoundEventArgs e)
    {
        var newDevice = new BluetoothDevice
        {
            DeviceName = e.Device.Name,
            MacAddress = e.Device.Address,
        };
        foundDevices.Add(newDevice, e.Device);
        if (e.Device?.Name is string name)
            if (DeviceFound != null)
                DeviceFound(this, new() { Device = newDevice });
    }

    public Task StopDiscovery()
    {
        bluetoothAdapter?.CancelDiscovery();
        return Task.CompletedTask;
    }

    private void BluetoothReceiver_DiscoveryFinished(object? sender, EventArgs e)
    {
        if (DiscoveryFinished != null)
            DiscoveryFinished(this, EventArgs.Empty);
    }

    public Task CreateDeviceBond(BluetoothDevice bluetoothDevice)
    {
        var device = foundDevices[bluetoothDevice];
        var success = device.CreateBond();
        if (!success)
            throw new InvalidOperationException();

        return Task.CompletedTask;
    }

    public Task<IEnumerable<BluetoothDevice>> GetBondedDevices()
    {
        if (bluetoothAdapter?.BondedDevices == null) return Task.FromResult<IEnumerable<BluetoothDevice>>([]);

        bondedDevices.Clear();
        foreach (var device in bluetoothAdapter.BondedDevices)
        {
            bondedDevices.Add(new()
            {
                DeviceName = device.Name,
                MacAddress = device.Address,
            }, device);
        }

        return Task.FromResult<IEnumerable<BluetoothDevice>>(bondedDevices.Keys);
    }

    private void BluetoothReceiver_BondStateChanged(object? sender, CustomCode.EventArgs.BondStateChangedEventArgs e)
    {
        if (BondStateChanged != null)
        {
            var values = foundDevices.Select(d => d.Value == e.Device).ToArray();
            var device = foundDevices.Single(x => x.Value.Address == e.Device.Address).Key;
            BondStateChanged(this, new()
            {
                Device = device,
                OldState = e.OldState switch
                {
                    Bond.None => BondState.None,
                    Bond.Bonding => BondState.Bonding,
                    Bond.Bonded => BondState.Bonded,
                    _ => BondState.None,
                },
                NewState = e.NewState switch
                {
                    Bond.None => BondState.None,
                    Bond.Bonding => BondState.Bonding,
                    Bond.Bonded => BondState.Bonded,
                    _ => BondState.None,
                },
            });
        }
    }

    public async Task ConnectToDevice(BluetoothDevice bluetoothDevice)
    {
        var device = bondedDevices.Single(x => x.Value.Address == bluetoothDevice.MacAddress).Value;
        var uuid = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
        var socket = device.CreateRfcommSocketToServiceRecord(uuid);
        await socket!.ConnectAsync();

        await Task.Delay(1000);
        await socket.OutputStream!.WriteAsync([0x1f], 0, 1);

        //await Task.Delay(1000);
        //await socket.OutputStream!.WriteAsync([0x14], 0, 1);

        //await Task.Delay(1000);
        //await socket.OutputStream!.WriteAsync([0x12], 0, 1);

        await Task.Delay(10000);
    }
}
