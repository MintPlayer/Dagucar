using Android.Bluetooth;
using Android.Content;
using Java.Util;
using AndroidBluetoothAdapter = Android.Bluetooth.BluetoothAdapter;
using AndroidBluetoothDevice = Android.Bluetooth.BluetoothDevice;

namespace MintPlayer.Maui.Bluetooth.Platforms.Android;

internal class BluetoothReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        switch (intent?.Action)
        {
            case AndroidBluetoothDevice.ActionFound:
                if (intent.GetParcelableExtra(AndroidBluetoothDevice.ExtraDevice) is AndroidBluetoothDevice device)
                    OnDeviceFound(new EventArgs.DeviceFoundEventArgs { Device = device });
                break;
            case AndroidBluetoothAdapter.ActionDiscoveryStarted:
                OnDiscoveryStarted(System.EventArgs.Empty);
                break;
            case AndroidBluetoothAdapter.ActionDiscoveryFinished:
                OnDiscoveryFinished(System.EventArgs.Empty);
                break;
            case AndroidBluetoothDevice.ActionBondStateChanged:
                if (intent.GetParcelableExtra(AndroidBluetoothDevice.ExtraDevice) is AndroidBluetoothDevice device2)
                {
                    //var oldState = (Bond)(int)intent.GetParcelableExtra(BluetoothDevice.ExtraPreviousBondState, );
                    //var newState = (Bond)(int)intent.GetParcelableExtra(BluetoothDevice.ExtraBondState);
                    var oldState = (Bond)(int)intent.GetParcelableExtra(BluetoothDevice.ExtraPreviousBondState, Java.Lang.Class.FromType(typeof(Java.Lang.Integer)));
                    var newState = (Bond)(int)intent.GetParcelableExtra(BluetoothDevice.ExtraBondState, Java.Lang.Class.FromType(typeof(Java.Lang.Integer)));
                    OnBondStateChanged(new EventArgs.BondStateChangedEventArgs { Device = device2, OldState = oldState, NewState = newState });
                }
                break;
            case AndroidBluetoothDevice.ActionUuid:
                if (intent.GetParcelableExtra(AndroidBluetoothDevice.ExtraUuid) is UUID uuid)
                    OnUUIDFetched(new EventArgs.UuidFetchedEventArgs { UUID = uuid });
                break;
        }
    }

    #region DeviceFound
    public event EventHandler<EventArgs.DeviceFoundEventArgs> DeviceFound;
    protected void OnDeviceFound(EventArgs.DeviceFoundEventArgs e)
    {
        if (DeviceFound != null)
            DeviceFound(this, e);
    }
    #endregion
    #region DiscoveryStarted
    public event EventHandler? DiscoveryStarted;
    protected void OnDiscoveryStarted(System.EventArgs e)
    {
        if (DiscoveryStarted != null)
            DiscoveryStarted(this, e);
    }
    #endregion
    #region DiscoveryFinished
    public event EventHandler? DiscoveryFinished;
    protected void OnDiscoveryFinished(System.EventArgs e)
    {
        if (DiscoveryFinished != null)
            DiscoveryFinished(this, e);
    }
    #endregion
    #region BondStateChanged
    public event EventHandler<EventArgs.BondStateChangedEventArgs>? BondStateChanged;
    protected void OnBondStateChanged(EventArgs.BondStateChangedEventArgs e)
    {
        if (BondStateChanged != null)
            BondStateChanged(this, e);
    }
    #endregion
    #region UuidFetched
    public event EventHandler<EventArgs.UuidFetchedEventArgs>? UuidFetched;
    protected void OnUUIDFetched(EventArgs.UuidFetchedEventArgs e)
    {
        if (UuidFetched != null)
            UuidFetched(this, e);
    }
    #endregion
}
