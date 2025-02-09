using Android.Bluetooth;
using Android.Content;
using Java.Util;
using static Android.Net.Nsd.NsdManager;

namespace Dagucar.Platforms.Android.CustomCode;

internal class BluetoothReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        switch (intent?.Action)
        {
            case BluetoothDevice.ActionFound:
                if (intent.GetParcelableExtra(BluetoothDevice.ExtraDevice) is BluetoothDevice device)
                    OnDeviceFound(new EventArgs.DeviceFoundEventArgs { Device = device });
                break;
            case BluetoothAdapter.ActionDiscoveryStarted:
                OnDiscoveryStarted(System.EventArgs.Empty);
                break;
            case BluetoothAdapter.ActionDiscoveryFinished:
                OnDiscoveryFinished(System.EventArgs.Empty);
                break;
            case BluetoothDevice.ActionBondStateChanged:
                if (intent.GetParcelableExtra(BluetoothDevice.ExtraDevice) is BluetoothDevice device2)
                {
                    //var oldState = (Bond)(int)intent.GetParcelableExtra(BluetoothDevice.ExtraPreviousBondState, );
                    //var newState = (Bond)(int)intent.GetParcelableExtra(BluetoothDevice.ExtraBondState);
                    var oldState = (Bond)(int)intent.GetParcelableExtra(BluetoothDevice.ExtraPreviousBondState, Java.Lang.Class.FromType(typeof(Java.Lang.Integer)));
                    var newState = (Bond)(int)intent.GetParcelableExtra(BluetoothDevice.ExtraBondState, Java.Lang.Class.FromType(typeof(Java.Lang.Integer)));
                    OnBondStateChanged(new EventArgs.BondStateChangedEventArgs { Device = device2, OldState = oldState, NewState = newState });
                }
                break;
            case BluetoothDevice.ActionUuid:
                if (intent.GetParcelableExtra(BluetoothDevice.ExtraUuid) is UUID uuid)
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
