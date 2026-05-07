using System.Collections.Generic;
using UnityEngine;

public class UnitySystemResolutionProvider : IResolutionProvider
{
    private const int StartIndex = 0;

    public List<Resolution> GetUniqueResolutions()
    {
        Resolution[] allResolutions = Screen.resolutions;
        List<Resolution> uniqueResolutions = new List<Resolution>();

        for (int i = StartIndex; i < allResolutions.Length; i++)
        {
            bool isDuplicate = false;

            for (int j = StartIndex; j < uniqueResolutions.Count; j++)
            {
                if (allResolutions[i].width == uniqueResolutions[j].width &&
                    allResolutions[i].height == uniqueResolutions[j].height)
                {
                    isDuplicate = true;
                    break;
                }
            }

            if (!isDuplicate)
            {
                uniqueResolutions.Add(allResolutions[i]);
            }
        }

        return uniqueResolutions;
    }

    public Resolution GetCurrentResolution()
    {
        return Screen.currentResolution;
    }

    public void ApplyResolution(Resolution resolution, bool isFullScreen)
    {
        Screen.SetResolution(resolution.width, resolution.height, isFullScreen);
    }
}