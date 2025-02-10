namespace MintPlayer.Maui.Bluetooth;

public class BluetoothDevice : IBluetoothDevice
{
    public string? DeviceName => throw new NotImplementedException();

    public string? MacAddress => throw new NotImplementedException();

    public Task CreateBond() => throw new NotImplementedException();

    public Task Connect() => throw new NotImplementedException();

    public Task Disconnect() => throw new NotImplementedException();

    public Task SendData(byte[] data) => throw new NotImplementedException();
}
