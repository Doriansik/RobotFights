using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

#region Interfaces
public interface IMusicController
{
    void PlayMenuMusic();
    void PlayGameMusic();
    void StopMusic();
}
#endregion

#region Classes
public class MusicManager : MonoBehaviour, IMusicController
{
    #region Constants
    private const float ZeroVolume = 0f;
    private const float DefaultTimerStart = 0f;
    private const float MinimumDurationLimit = 0.01f;
    private const float DefaultFadeDuration = 1f;
    private const float DefaultTargetVolume = 1f;
    private const float MinVolumeRange = 0f;
    private const float MaxVolumeRange = 1f;
    #endregion

    #region Serialized Fields
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private float fadeDuration = DefaultFadeDuration;
    [Range(MinVolumeRange, MaxVolumeRange)][SerializeField] private float targetVolume = DefaultTargetVolume;
    [SerializeField] private List<string> menuSceneNames;
    [SerializeField] private List<string> gameSceneNames;
    #endregion

    #region Private Fields
    private static MusicManager instance;
    private Coroutine fadeCoroutine;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        ManageSingletonInstance();
        InitializeAudioSource();
    }

    private void Start()
    {
        EvaluateCurrentScene();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #endregion

    #region Public Methods
    public void PlayMenuMusic()
    {
        TransitionToTrack(menuMusic);
    }

    public void PlayGameMusic()
    {
        TransitionToTrack(gameMusic);
    }

    public void StopMusic()
    {
        ResetFadeCoroutine();
        musicSource.Stop();
        musicSource.clip = null;
    }
    #endregion

    #region Private Methods
    private void ManageSingletonInstance()
    {
        if (instance == null)
        {
            instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSource()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }
        musicSource.loop = true;
        musicSource.volume = targetVolume;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EvaluateSceneMusic(scene.name);
    }

    private void EvaluateCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        EvaluateSceneMusic(currentSceneName);
    }

    private void EvaluateSceneMusic(string sceneName)
    {
        if (IsSceneInList(sceneName, menuSceneNames))
        {
            PlayMenuMusic();
        }
        else if (IsSceneInList(sceneName, gameSceneNames))
        {
            PlayGameMusic();
        }
    }

    private bool IsSceneInList(string sceneName, List<string> sceneList)
    {
        if (sceneList == null)
        {
            return false;
        }

        foreach (string name in sceneList)
        {
            if (string.Equals(name?.Trim(), sceneName?.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    private void TransitionToTrack(AudioClip clip)
    {
        if (musicSource.clip == clip)
        {
            return;
        }

        ResetFadeCoroutine();

        if (gameObject.activeInHierarchy)
        {
            fadeCoroutine = StartCoroutine(FadeToTrackRoutine(clip));
        }
        else
        {
            ExecuteImmediateTrackChange(clip);
        }
    }

    private void ResetFadeCoroutine()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }

    private void ExecuteImmediateTrackChange(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.volume = targetVolume;
        if (clip != null)
        {
            musicSource.Play();
        }
    }

    private IEnumerator FadeToTrackRoutine(AudioClip nextClip)
    {
        float startVolume = musicSource.volume;
        float timer = DefaultTimerStart;
        float calculatedDuration = Mathf.Max(fadeDuration, MinimumDurationLimit);

        while (timer < calculatedDuration)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, ZeroVolume, timer / calculatedDuration);
            yield return null;
        }

        musicSource.clip = nextClip;

        if (nextClip != null)
        {
            musicSource.Play();
        }

        timer = DefaultTimerStart;

        while (timer < calculatedDuration)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(ZeroVolume, targetVolume, timer / calculatedDuration);
            yield return null;
        }
    }
    #endregion
}
#endregion