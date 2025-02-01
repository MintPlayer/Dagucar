using Android.Bluetooth;
using Dagucar.Platforms.Android.CustomCode;
using Dagucar.Services;
using System.Collections.ObjectModel;

namespace Dagucar.Platforms.Android.Services;

internal class BluetoothService : IBluetoothService
{
    private readonly BluetoothAdapter? bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
    private readonly global::Android.Content.Context context;
    //CustomCode.BluetoothReceiver? bluetoothReceiver = new();

    public ObservableCollection<string> BluetoothDevices { get; } = new();

    public BluetoothService()
    {
        context = global::Microsoft.Maui.ApplicationModel.Platform.CurrentActivity
            ?? global::Microsoft.Maui.MauiApplication.Context;

        if (bluetoothAdapter == null || !bluetoothAdapter.IsEnabled)
            throw new Exception("Bluetooth not available/enabled");

        //bluetoothReceiver = new BluetoothReceiver();

        //bluetoothReceiver.DiscoveryStarted += BluetoothReceiver_DiscoveryStarted;
        //bluetoothReceiver.DiscoveryFinished += BluetoothReceiver_DiscoveryFinished;
        //bluetoothReceiver.DeviceFound += BluetoothReceiver_DeviceFound;
        //bluetoothReceiver.UuidFetched += BluetoothReceiver_UuidFetched;

        //foreach (var action in new[] { BluetoothDevice.ActionFound, BluetoothAdapter.ActionDiscoveryStarted, BluetoothAdapter.ActionDiscoveryFinished, BluetoothDevice.ActionBondStateChanged })
        //    context.RegisterReceiver(bluetoothReceiver, new global::Android.Content.IntentFilter(action));
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

    public Task StartDiscovery()
    {
        BluetoothDevices.Clear();
        bluetoothAdapter?.StartDiscovery();
        return Task.CompletedTask;
    }

    private void BluetoothReceiver_UuidFetched(object? sender, Platforms.Android.CustomCode.EventArgs.UuidFetchedEventArgs e)
    {
    }

    private void BluetoothReceiver_DeviceFound(object? sender, Platforms.Android.CustomCode.EventArgs.DeviceFoundEventArgs e)
    {
        if (e.Device?.Name is string name)
            BluetoothDevices.Add(name);
    }

    private void BluetoothReceiver_DiscoveryFinished(object? sender, EventArgs e)
    {
    }

    private void BluetoothReceiver_DiscoveryStarted(object? sender, EventArgs e)
    {
    }
}
