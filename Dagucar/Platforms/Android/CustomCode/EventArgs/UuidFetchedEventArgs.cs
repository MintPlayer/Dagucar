using Java.Util;

namespace Dagucar.Platforms.Android.CustomCode.EventArgs;

internal sealed class UuidFetchedEventArgs : System.EventArgs
{
    public required UUID UUID { get; init; }
}
