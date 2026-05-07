public interface ISettingsStorage
{
    void SaveInt(string key, int value);
    int LoadInt(string key, int defaultValue);
    bool HasKey(string key);
}