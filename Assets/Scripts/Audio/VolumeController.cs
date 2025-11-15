using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Simple master-volume manager that preserves each AudioSource's original volume
/// and applies a master multiplier so per-source relative volumes remain the same.
/// </summary>
public class VolumeController : MonoBehaviour
{
    public static VolumeController Instance { get; private set; }

    private float masterVolume = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // When new scenes load, make sure master is applied to newly created AudioSources
        ApplyMasterToAll();
    }

    /// <summary>
    /// Set the master volume (0..1) and apply it to all discovered AudioSources.
    /// </summary>
    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
        // Use AudioListener volume for global master control. This is safer than
        // overwriting each AudioSource.volume which can interfere with streaming/scheduled playback
        // and cause audible cutoffs on some platforms/clip types.
        AudioListener.volume = masterVolume;
    }
    private void ApplyMasterToAll()
    {
        // Kept for compatibility with sceneLoaded hook; currently we rely on AudioListener.volume
        // so no per-source modification is necessary. Leaving this method in case future
        // behavior (e.g., AudioMixer integration) is desired.
    }

    /// <summary>
    /// If you dynamically create AudioSources and want to ensure their base volume
    /// is recorded immediately, call this.
    /// </summary>
    public void RegisterAudioSource(AudioSource src)
    {
        // No-op for now. If you need per-source registration (to store original volumes
        // for later mute/solo behaviors) we can implement it. Relying on AudioListener.volume
        // avoids many pitfalls with playback interruption.
    }
}
