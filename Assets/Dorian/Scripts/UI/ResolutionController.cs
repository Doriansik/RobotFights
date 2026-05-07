using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResolutionController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private IResolutionProvider resolutionProvider;
    private ISettingsStorage settingsStorage;
    private List<Resolution> availableResolutions;
    private readonly List<string> resolutionOptions = new List<string>();

    private const string ResolutionSeparator = " x ";
    private const string SavedResolutionWidthKey = "SavedResolutionWidth";
    private const string SavedResolutionHeightKey = "SavedResolutionHeight";
    private const int DefaultIndex = 0;

    private void Awake()
    {
        resolutionProvider = new UnitySystemResolutionProvider();
        settingsStorage = new PlayerPrefsSettingsStorage();
    }

    private void Start()
    {
        InitializeDropdown();
    }

    private void InitializeDropdown()
    {
        availableResolutions = resolutionProvider.GetUniqueResolutions();
        resolutionDropdown.ClearOptions();

        int currentResolutionIndex = DefaultIndex;
        bool hasSavedResolution = settingsStorage.HasKey(SavedResolutionWidthKey) && settingsStorage.HasKey(SavedResolutionHeightKey);

        int targetWidth = hasSavedResolution
            ? settingsStorage.LoadInt(SavedResolutionWidthKey, DefaultIndex)
            : resolutionProvider.GetCurrentResolution().width;

        int targetHeight = hasSavedResolution
            ? settingsStorage.LoadInt(SavedResolutionHeightKey, DefaultIndex)
            : resolutionProvider.GetCurrentResolution().height;

        for (int i = DefaultIndex; i < availableResolutions.Count; i++)
        {
            string optionText = availableResolutions[i].width + ResolutionSeparator + availableResolutions[i].height;
            resolutionOptions.Add(optionText);

            if (availableResolutions[i].width == targetWidth && availableResolutions[i].height == targetHeight)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        if (hasSavedResolution)
        {
            resolutionProvider.ApplyResolution(availableResolutions[currentResolutionIndex], Screen.fullScreen);
        }

        resolutionDropdown.onValueChanged.AddListener(OnResolutionSelected);
    }

    private void OnResolutionSelected(int index)
    {
        Resolution selectedResolution = availableResolutions[index];
        resolutionProvider.ApplyResolution(selectedResolution, Screen.fullScreen);

        settingsStorage.SaveInt(SavedResolutionWidthKey, selectedResolution.width);
        settingsStorage.SaveInt(SavedResolutionHeightKey, selectedResolution.height);
    }
}