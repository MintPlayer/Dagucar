using Android.Bluetooth;

namespace Dagucar.Platforms.Android.CustomCode.EventArgs;

internal sealed class DeviceFoundEventArgs : System.EventArgs
{
    public required BluetoothDevice Device { get; init; }
}
