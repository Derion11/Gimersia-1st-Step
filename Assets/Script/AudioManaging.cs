using UnityEngine;
using System.Collections.Generic;

public class AudioManaging : MonoBehaviour
{
    public static AudioManaging Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    public List<AudioClip> bgmClips;
    public List<AudioClip> sfxClips;

    private Dictionary<string, AudioClip> bgmDict = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> sfxDict = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Ensure AudioSources exist
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        // Build dictionaries
        foreach (var clip in bgmClips)
            if (clip != null) bgmDict[clip.name] = clip;

        foreach (var clip in sfxClips)
            if (clip != null) sfxDict[clip.name] = clip;
    }

    #region BGM
    public void PlayBGM(string name, bool loop = true)
    {
        if (bgmSource == null)
        {
            Debug.LogError("[AudioManaging] Missing BGM AudioSource!");
            return;
        }

        if (!bgmDict.TryGetValue(name, out AudioClip clip))
        {
            Debug.LogWarning($"[AudioManaging] BGM '{name}' not found!");
            return;
        }

        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource != null)
            bgmSource.Stop();
    }
    #endregion

    #region SFX
    public void PlaySFX(string name, float volumeScale = 1f)
    {
        if (sfxSource == null)
        {
            Debug.LogError("[AudioManaging] Missing SFX AudioSource!");
            return;
        }

        if (!sfxDict.TryGetValue(name, out AudioClip clip))
        {
            Debug.LogWarning($"[AudioManaging] SFX '{name}' not found!");
            return;
        }

        sfxSource.PlayOneShot(clip, Mathf.Clamp01(volumeScale));
    }
    #endregion
}
