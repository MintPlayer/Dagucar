using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using Dagucar.Platforms.Android.CustomCode;
using Dagucar.Platforms.Android.CustomCode.EventArgs;

namespace Dagucar
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //var bluetoothReceiver = new BluetoothReceiver();

            //bluetoothReceiver.DiscoveryStarted += BluetoothReceiver_DiscoveryStarted;
            //bluetoothReceiver.DiscoveryFinished += BluetoothReceiver_DiscoveryFinished;
            //bluetoothReceiver.DeviceFound += BluetoothReceiver_DeviceFound;
            //bluetoothReceiver.UuidFetched += BluetoothReceiver_UuidFetched;

            //foreach (var action in new[] { BluetoothDevice.ActionFound, BluetoothAdapter.ActionDiscoveryStarted, BluetoothAdapter.ActionDiscoveryFinished, BluetoothDevice.ActionBondStateChanged })
            //    RegisterReceiver(bluetoothReceiver, new global::Android.Content.IntentFilter(action));
            //RegisterReceiver(bluetoothReceiver, new global::Android.Content.IntentFilter(BluetoothAdapter.ActionDiscoveryStarted));
        }

        //private void BluetoothReceiver_UuidFetched(object? sender, UuidFetchedEventArgs e)
        //{
        //}

        //private void BluetoothReceiver_DeviceFound(object? sender, DeviceFoundEventArgs e)
        //{

        //}

        //private void BluetoothReceiver_DiscoveryFinished(object? sender, EventArgs e)
        //{
        //    isDiscovering = false;
        //}

        //private void BluetoothReceiver_DiscoveryStarted(object? sender, EventArgs e)
        //{
        //}

        private bool isDiscovering = false;
        //protected override void OnStart()
        //{
        //    base.OnStart();
        //    ActivityCompat.RequestPermissions(this, [
        //        Android.Manifest.Permission.Bluetooth,
        //        Android.Manifest.Permission.BluetoothAdmin,
        //        Android.Manifest.Permission.BluetoothAdvertise,
        //        Android.Manifest.Permission.BluetoothConnect,
        //        Android.Manifest.Permission.BluetoothPrivileged,
        //        Android.Manifest.Permission.BluetoothScan,
        //        Android.Manifest.Permission.AccessCoarseLocation,
        //        Android.Manifest.Permission.AccessFineLocation,
        //        //"android.hardware.sensor.accelerometer"
        //    ], 1);
        //}

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Microsoft.Maui.ApplicationModel.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            //if (!isDiscovering)
            //{
            //    isDiscovering = true;
                BluetoothAdapter.DefaultAdapter!.StartDiscovery();
            //}
        }
    }
}
