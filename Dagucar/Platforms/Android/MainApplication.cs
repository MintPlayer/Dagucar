using Android.App;
using Android.Runtime;
using Dagucar.Platforms.Android.Services;
using Dagucar.Services;

namespace Dagucar
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp()
        {
            var app = MauiProgram.CreateMauiApp();
            app.Services.AddSingleton<IBluetoothService, BluetoothService>();
            app.Services.AddSingleton<IFlagService, FlagService>();
            return app.Build();
        }
    }
}
