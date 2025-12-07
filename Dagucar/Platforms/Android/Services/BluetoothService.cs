using Android.Bluetooth;
using AndroidX.Core.App;
using Dagucar.Platforms.Android.CustomCode;
using Dagucar.Services;
using System.Collections.ObjectModel;
using Java.Util;
using System.Threading;
using Android.Runtime;
using AndroidBluetoothDevice = Android.Bluetooth.BluetoothDevice;
using DagucarBluetoothDevice = Dagucar.Services.BluetoothDevice;

namespace Dagucar.Platforms.Android.Services;

internal class BluetoothService : IBluetoothService
{
    private readonly BluetoothAdapter? bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
    private readonly global::Android.Content.Context context;
    private readonly IFlagService flagService;
    private BluetoothGatt? legoGatt;
    private BluetoothGattCharacteristic? legoWriteCharacteristic;
    private TaskCompletionSource<bool>? legoReadyTcs;
    private TaskCompletionSource<bool>? writeCompletionTcs;
    private readonly SemaphoreSlim writeLock = new(1, 1);
    private bool legoConnected;
    private const string LegoServiceUuid = "00001623-1212-efde-1623-785feabcd123";
    private const string LegoCharacteristicUuid = "00001624-1212-efde-1623-785feabcd123";
    private const byte DrivePort = 0x00;   // Typically left/drive motor on TopGear
    private const byte SteerPort = 0x01;   // Steering motor
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

    public bool IsLegoConnected => legoConnected;

    public Task<bool> CreateBond(DagucarBluetoothDevice ddevice)
    {
        // LEGO Control+ hubs (Technic Hub) do not require or accept classic bonding; allow connect without bonding.
        if (!string.IsNullOrWhiteSpace(ddevice.Name) &&
            ddevice.Name.Contains("Technic Hub", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(true);
        }

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

    public async Task<bool> ConnectToLegoAsync(DagucarBluetoothDevice ddevice, CancellationToken cancellationToken = default)
    {
        await DisconnectLegoAsync();

        legoReadyTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
        var device = BluetoothAdapter.DefaultAdapter!.GetRemoteDevice(ddevice.Address);
        legoGatt = device.ConnectGatt(context, false, new LegoGattCallback(this));

        using (cancellationToken.Register(() => legoReadyTcs.TrySetCanceled(cancellationToken)))
        {
            var completed = await Task.WhenAny(legoReadyTcs.Task, Task.Delay(TimeSpan.FromSeconds(10), cancellationToken));
            if (completed != legoReadyTcs.Task)
                return false;

            legoConnected = legoReadyTcs.Task.Status == TaskStatus.RanToCompletion && legoReadyTcs.Task.Result;
            if (legoConnected)
            {
                // Send an initial stop to ensure motors are idle
                await SendLegoControlAsync(0, 0, cancellationToken);
            }

            return legoConnected;
        }
    }

    public async Task SendLegoControlAsync(int drivePower, int steerPower, CancellationToken cancellationToken = default)
    {
        if (!legoConnected || legoGatt == null || legoWriteCharacteristic == null)
            return;

        var drive = (sbyte)Math.Clamp(drivePower, -100, 100);
        var steer = (sbyte)Math.Clamp(steerPower, -100, 100);

        // Drive motor (forward/backward)
        await WriteLegoCommandAsync(BuildMotorCommand(DrivePort, drive), cancellationToken);
        // Steering motor (left/right)
        await WriteLegoCommandAsync(BuildMotorCommand(SteerPort, steer), cancellationToken);
    }

    public async Task DisconnectLegoAsync()
    {
        try
        {
            // Stop motors so the car does not continue after disconnect
            await SendLegoControlAsync(0, 0);
        }
        catch
        {
            // Ignore failures during shutdown
        }

        legoConnected = false;
        legoWriteCharacteristic = null;

        if (legoGatt != null)
        {
            legoGatt.Disconnect();
            legoGatt.Close();
            legoGatt.Dispose();
            legoGatt = null;
        }
    }

    internal void HandleLegoReady(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic)
    {
        legoGatt = gatt;
        legoWriteCharacteristic = characteristic;
        legoWriteCharacteristic.WriteType = GattWriteType.NoResponse;
        legoConnected = true;
        legoReadyTcs?.TrySetResult(true);
    }

    internal void HandleLegoDisconnected()
    {
        legoConnected = false;
        legoReadyTcs?.TrySetResult(false);
    }

    internal void HandleWriteComplete(bool success)
    {
        writeCompletionTcs?.TrySetResult(success);
    }

    private async Task<bool> WriteLegoCommandAsync(byte[] command, CancellationToken cancellationToken)
    {
        if (legoGatt == null || legoWriteCharacteristic == null)
            return false;

        await writeLock.WaitAsync(cancellationToken);
        try
        {
            writeCompletionTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
            legoWriteCharacteristic.SetValue(command);
            var result = legoGatt.WriteCharacteristic(legoWriteCharacteristic);

            if (!result)
            {
                writeCompletionTcs.TrySetResult(false);
            }

            using (cancellationToken.Register(() => writeCompletionTcs.TrySetCanceled(cancellationToken)))
            {
                var completed = await Task.WhenAny(writeCompletionTcs.Task, Task.Delay(TimeSpan.FromSeconds(5), cancellationToken));
                if (completed != writeCompletionTcs.Task)
                    return false;
                return writeCompletionTcs.Task.Result;
            }
        }
        finally
        {
            writeLock.Release();
        }
    }

    private static byte[] BuildMotorCommand(byte port, sbyte power)
    {
        // LEGO LWP3 "WriteDirectModeData" message: [len][hubId][0x81][port][startup/completion][0x51][mode][value]
        return
        [
            0x09, 0x00,             // length (little endian, includes this header)
            0x00,                   // hub id (0)
            0x81,                   // Port Output Command
            port,                   // Port
            0x11,                   // Start execution + feedback
            0x51,                   // WriteDirectModeData (set speed)
            0x00,                   // Mode 0 (duty cycle / speed)
            unchecked((byte)power)
        ];
    }

    private sealed class LegoGattCallback : BluetoothGattCallback
    {
        private readonly BluetoothService parent;

        public LegoGattCallback(BluetoothService parent)
        {
            this.parent = parent;
        }

        public override void OnConnectionStateChange(BluetoothGatt gatt, GattStatus status, ProfileState newState)
        {
            base.OnConnectionStateChange(gatt, status, newState);

            if (status != GattStatus.Success || newState != ProfileState.Connected)
            {
                parent.HandleLegoDisconnected();
                return;
            }

            gatt.DiscoverServices();
        }

        public override void OnServicesDiscovered(BluetoothGatt gatt, GattStatus status)
        {
            base.OnServicesDiscovered(gatt, status);

            if (status != GattStatus.Success)
            {
                parent.HandleLegoDisconnected();
                return;
            }

            var service = gatt.GetService(UUID.FromString(LegoServiceUuid));
            var characteristic = service?.GetCharacteristic(UUID.FromString(LegoCharacteristicUuid));

            if (service == null || characteristic == null)
            {
                parent.HandleLegoDisconnected();
                return;
            }

            gatt.SetCharacteristicNotification(characteristic, true);
            parent.HandleLegoReady(gatt, characteristic);
        }

        public override void OnCharacteristicWrite(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, [GeneratedEnum] GattStatus status)
        {
            base.OnCharacteristicWrite(gatt, characteristic, status);
            parent.HandleWriteComplete(status == GattStatus.Success);
        }
    }
}
