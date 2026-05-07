using UnityEngine;

public class PlayerPrefsSettingsStorage : ISettingsStorage
{
    public void SaveInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }

    public int LoadInt(string key, int defaultValue)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }
}