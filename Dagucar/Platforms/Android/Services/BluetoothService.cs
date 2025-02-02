using Android.Bluetooth;
using AndroidX.Core.App;
using Dagucar.Platforms.Android.CustomCode;
using Dagucar.Services;
using System.Collections.ObjectModel;

namespace Dagucar.Platforms.Android.Services;

internal class BluetoothService : IBluetoothService
{
    private readonly BluetoothAdapter? bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
    private readonly global::Android.Content.Context context;
    CustomCode.BluetoothReceiver? bluetoothReceiver = new();

    public bool IsDiscovering { get; private set; }

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

        foreach (var action in new[] { BluetoothDevice.ActionFound, BluetoothAdapter.ActionDiscoveryStarted, BluetoothAdapter.ActionDiscoveryFinished, BluetoothDevice.ActionBondStateChanged })
            context.RegisterReceiver(bluetoothReceiver, new global::Android.Content.IntentFilter(action));
    }

    public void Dispose()
    {
        //if (bluetoothReceiver != null)
        //{
        //    foreach (var action in new[] { BluetoothDevice.ActionFound, BluetoothAdapter.ActionDiscoveryStarted, BluetoothAdapter.ActionDiscoveryFinished, BluetoothDevice.ActionBondStateChanged })
        //        context.UnregisterReceiver(bluetoothReceiver);

        //    bluetoothReceiver.DiscoveryStarted -= BluetoothReceiver_DiscoveryStarted;
        //    bluetoothReceiver.DiscoveryFinished -= BluetoothReceiver_DiscoveryFinished;
        //    bluetoothReceiver.DeviceFound -= BluetoothReceiver_DeviceFound;
        //    bluetoothReceiver.UuidFetched -= BluetoothReceiver_UuidFetched;

        //    bluetoothReceiver.Dispose();
        //    bluetoothReceiver = null;
        //}
    }

    private Func<string, Task> deviceFound;
    private Func<Task> discoveryFinished;
    public Task StartDiscovery(Func<string, Task> deviceFound, Func<Task> discoveryFinished)
    {
        if (IsDiscovering) throw new InvalidOperationException();

        IsDiscovering = true;
        this.deviceFound = deviceFound;
        this.discoveryFinished = discoveryFinished;

        //BluetoothDevices.Clear();
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

    private void BluetoothReceiver_UuidFetched(object? sender, Platforms.Android.CustomCode.EventArgs.UuidFetchedEventArgs e)
    {
    }

    private async void BluetoothReceiver_DeviceFound(object? sender, Platforms.Android.CustomCode.EventArgs.DeviceFoundEventArgs e)
    {
        if (e.Device?.Name is string name)
            await deviceFound(name);
            //BluetoothDevices.Add(name);
    }

    private void BluetoothReceiver_DiscoveryFinished(object? sender, EventArgs e)
    {
        IsDiscovering = false;
        discoveryFinished();
    }

    private void BluetoothReceiver_DiscoveryStarted(object? sender, EventArgs e)
    {
    }
}
