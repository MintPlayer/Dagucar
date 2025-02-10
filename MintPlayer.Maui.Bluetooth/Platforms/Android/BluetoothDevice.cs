using Java.Util;
using AndroidBluetoothAdapter = Android.Bluetooth.BluetoothAdapter;
using AndroidBluetoothDevice = Android.Bluetooth.BluetoothDevice;

namespace MintPlayer.Maui.Bluetooth;

public class BluetoothDevice : IBluetoothDevice
{
    private readonly AndroidBluetoothDevice device;
    private Android.Bluetooth.BluetoothSocket? socket;
    public BluetoothDevice(AndroidBluetoothDevice device)
    {
        this.device = device;
    }

    public string? DeviceName => device.Name;

    public string? MacAddress => device.Address;

    public Task CreateBond()
    {
        device.CreateBond();
        return Task.CompletedTask;
    }

    public async Task Connect()
    {
        var uuid = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
        socket = device.CreateRfcommSocketToServiceRecord(uuid);
        await socket!.ConnectAsync();
    }

    public Task Disconnect()
    {
        if (socket != null)
        {
            socket.Close();
            socket.Dispose();
        }

        return Task.CompletedTask;
    }

    public async Task SendData(byte[] data)
    {
        await socket!.InputStream!.WriteAsync(data, 0, data.Length);
    }
}
