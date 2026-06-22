using System.Collections;
using UnityEngine;

public class VideoPlayerOFF : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField] private GameObject videoPlayer;
    [SerializeField] private float timeToOFF;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        StartCoroutine(DisableVideoPlayerRoutine());
    }

    #endregion

    #region Private Methods

    private IEnumerator DisableVideoPlayerRoutine()
    {
        float elapsedTime = 0f;
        int lastLoggedSecond = 0;

        while (elapsedTime < timeToOFF)
        {
            elapsedTime += Time.unscaledDeltaTime;
            int currentSecond = Mathf.FloorToInt(elapsedTime);

            if (currentSecond > lastLoggedSecond)
            {
                Debug.Log($"Up³ynê³o sekund: {currentSecond}");
                lastLoggedSecond = currentSecond;
            }

            yield return null;
        }

        if (videoPlayer != null)
        {
            videoPlayer.SetActive(false);
        }
    }

    #endregion
}