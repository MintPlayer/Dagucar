#if ANDROID
using Android.Bluetooth;
using Java.Util;
using AndroidBluetoothDevice = Android.Bluetooth.BluetoothDevice;
#endif

namespace Dagucar.Services;

public class BluetoothDevice
{
    public BluetoothDevice()
    {
    }

    public async Task Connect()
    {
#if ANDROID
        var uuid = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
        var socket = InternalDevice!.CreateRfcommSocketToServiceRecord(uuid);
        await socket!.ConnectAsync();
#elif IOS

#elif MACCATALYST

#elif WINDOWS

#endif
    }

#if ANDROID
    internal AndroidBluetoothDevice? InternalDevice { get; init; }
    private BluetoothSocket? bluetoothSocket;

    public string? DeviceName => InternalDevice?.Name;
    public string? MacAddress => InternalDevice?.Address;
#elif IOS

#elif MACCATALYST

#elif WINDOWS

#endif
}
