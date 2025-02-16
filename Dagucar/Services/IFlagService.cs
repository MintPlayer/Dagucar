namespace Dagucar.Services;

public interface IFlagService
{
    bool TryGetFlag(string key, out string? value);
    string? GetFlag(string key);
    void SetFlag(string key, string value);
    void ClearFlag(string key);
}
