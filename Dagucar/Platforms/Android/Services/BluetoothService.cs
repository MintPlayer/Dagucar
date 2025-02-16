﻿using Android.Bluetooth;
using AndroidX.Core.App;
using Dagucar.Platforms.Android.CustomCode;
using Dagucar.Services;
using System.Collections.ObjectModel;
using AndroidBluetoothDevice = Android.Bluetooth.BluetoothDevice;
using DagucarBluetoothDevice = Dagucar.Services.BluetoothDevice;

namespace Dagucar.Platforms.Android.Services;

internal class BluetoothService : IBluetoothService
{
    private readonly BluetoothAdapter? bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
    private readonly global::Android.Content.Context context;
    private readonly IFlagService flagService;
    CustomCode.BluetoothReceiver? bluetoothReceiver = new();

    //public bool IsDiscovering { get; private set; }

    public BluetoothService(IFlagService flagService)
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
        this.flagService = flagService;
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

    private Func<DagucarBluetoothDevice, Task> deviceFound;
    private Func<Task> discoveryFinished;
    public async Task<bool> StartDiscovery(Func<DagucarBluetoothDevice, Task> deviceFound, Func<Task> discoveryFinished)
    {
        //if (IsDiscovering) throw new InvalidOperationException();

        //IsDiscovering = true;

        // Verify that we're not already in discovery mode
        if (!flagService.TryGetFlag("BluetoothService.StartDiscovery", out _))
        {
            this.deviceFound = deviceFound;
            this.discoveryFinished = discoveryFinished;
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

            await Task.Run(async () =>
            {
                while (!flagService.TryGetFlag("BluetoothService.StartDiscovery", out var flag) && flag != "1")
                {
                    await Task.Delay(100);
                }
            });

            var success = BluetoothAdapter.DefaultAdapter!.StartDiscovery();
            return success;
        }

        return false;
    }

    public Task<bool> StopDiscovery()
    {
        var result = BluetoothAdapter.DefaultAdapter!.CancelDiscovery();
        return Task.FromResult(result);
    }

    private void BluetoothReceiver_UuidFetched(object? sender, Platforms.Android.CustomCode.EventArgs.UuidFetchedEventArgs e)
    {
    }

    private async void BluetoothReceiver_DeviceFound(object? sender, Platforms.Android.CustomCode.EventArgs.DeviceFoundEventArgs e)
    {
        if (e.Device is AndroidBluetoothDevice device && !string.IsNullOrEmpty(device.Name))
        {
            await deviceFound(new() { Name = device.Name!, Address = device.Address! });
        }
        //BluetoothDevices.Add(name);
    }

    private void BluetoothReceiver_DiscoveryFinished(object? sender, EventArgs e)
    {
        //IsDiscovering = false;
        flagService.ClearFlag("BluetoothService.StartDiscovery");
        discoveryFinished();
    }

    private void BluetoothReceiver_DiscoveryStarted(object? sender, EventArgs e)
    {
    }

    public Task<bool> CreateBond(DagucarBluetoothDevice ddevice)
    {
        var device = BluetoothAdapter.DefaultAdapter!.GetRemoteDevice(ddevice.Address);
        device.CreateBond();
        return Task.FromResult(device != null);
    }

    private void BluetoothReceiver_BondStateChanged(object? sender, CustomCode.EventArgs.BondStateChangedEventArgs e)
    {

    }

    public Task<IEnumerable<DagucarBluetoothDevice>> GetBondedDevices()
    {
        var devices = BluetoothAdapter.DefaultAdapter!.BondedDevices;
        var result = devices.Select(d => new DagucarBluetoothDevice { Name = d.Name, Address = d.Address });
        return Task.FromResult(result);
    }
}
