using Dagucar.Services;

namespace Dagucar.Platforms.Android.Services;

internal class FlagService : IFlagService
{
    private readonly Dictionary<string, string> values = new();

    public void ClearFlag(string key)
    {
        if (values.ContainsKey(key))
            values.Remove(key);
    }

    public string? GetFlag(string key)
    {
        if (values.TryGetValue(key, out var value))
            return value;
        return null;
    }

    public void SetFlag(string key, string value)
    {
        values[key] = value;
    }

    public bool TryGetFlag(string key, out string? value)
    {
        return values.TryGetValue(key, out value);
    }
}
