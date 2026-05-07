using System.Collections.Generic;
using UnityEngine;

public interface IResolutionProvider
{
    List<Resolution> GetUniqueResolutions();
    Resolution GetCurrentResolution();
    void ApplyResolution(Resolution resolution, bool isFullScreen);
}