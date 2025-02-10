using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using WindowsBluetoothAdapter = Windows.Devices.Bluetooth.BluetoothAdapter;
using WindowsBluetoothDevice = Windows.Devices.Bluetooth.BluetoothLEDevice;

namespace MintPlayer.Maui.Bluetooth;

public class BluetoothDevice : IBluetoothDevice
{
    private readonly WindowsBluetoothDevice device;
    //private BluetoothLEDevice? bleConnection;
    //private readonly DeviceInformation info;
    private StreamSocket? socket;

    public BluetoothDevice(WindowsBluetoothDevice device)
    {
        this.device = device;
    }
    public string? DeviceName => device.Name;

    public string? MacAddress => string.Join(":", BitConverter.GetBytes(device.BluetoothAddress)
        .Reverse()
        .Take(6)  // Take only the first 6 bytes
        .Select(b => b.ToString("X2")));

    public Task CreateBond() => throw new NotImplementedException();

    public async Task Connect()
    {
        var commService = await RfcommDeviceService.FromIdAsync(device.BluetoothDeviceId.Id);
        socket = new StreamSocket();
        await socket.ConnectAsync(commService.ConnectionHostName, commService.ConnectionServiceName);


        //bleConnection = await BluetoothLEDevice.FromBluetoothAddressAsync(device.BluetoothAddress);
        //if (bleConnection == null)
        //    throw new InvalidOperationException();
    }

    public Task Disconnect()
    {
        if (socket != null)
            socket.Dispose();

        return Task.CompletedTask;
    }

    public Task SendData(byte[] data)
    {
        using var writer = new DataWriter(socket.OutputStream);
        writer.WriteBytes(data);
        return Task.CompletedTask;
    }
}
