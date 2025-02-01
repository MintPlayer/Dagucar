using Android.Bluetooth;

namespace Dagucar.Platforms.Android.CustomCode.EventArgs;

internal sealed class BondStateChangedEventArgs : System.EventArgs
{
    public required BluetoothDevice Device { get; init; }
    public required Bond OldState { get; init; }
    public required Bond NewState { get; init; }
}
