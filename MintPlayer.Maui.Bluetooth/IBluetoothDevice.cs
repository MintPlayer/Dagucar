namespace MintPlayer.Maui.Bluetooth;

public interface IBluetoothDevice
{
    Task CreateBond();
    Task Connect();
    Task Disconnect();
    Task SendData(byte[] data);

    string? DeviceName { get; }
    string? MacAddress { get; }
}
