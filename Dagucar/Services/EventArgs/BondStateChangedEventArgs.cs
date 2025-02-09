namespace Dagucar.Services.EventArgs;

public class BondStateChangedEventArgs : System.EventArgs
{
    public required BluetoothDevice Device { get; init; }
    public required BondState OldState { get; init; }
    public required BondState NewState { get; init; }
}

public enum BondState
{
    None,
    Bonding,
    Bonded,
}
