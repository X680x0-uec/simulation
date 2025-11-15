using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleStartButton : MonoBehaviour
{


    [SerializeField] private string targetScene = "ExplainScene1";
    [Header("Optional sound played once when Start is clicked")]
    [Tooltip("If set, this AudioClip will be played once when the button is clicked. If empty, the script will try to load '決定' from Resources.")]
    [SerializeField] private AudioClip decideClip;
    [Tooltip("Optional AudioSource to use for playback. If not set, a temporary AudioSource GameObject will be created and marked DontDestroyOnLoad so the sound continues across scene load.")]
    [SerializeField] private AudioSource audioSource;
    // internal guard to ensure the clip is played only once per application run
    private bool hasPlayedDecide = false;
    
    public void OnStartButtonClicked()
    {
        Debug.Log("Start Button Clicked");
        // Play decide sound once
        TryPlayDecideSoundOnce();

        if (IsSceneInBuild(targetScene))
        {
            SceneManager.LoadScene(targetScene);
        }
        else
        {
            Debug.LogError($"Scene '{targetScene}' is not registered in Build Settings. Add it before trying to load.");
        }
    }

    private void TryPlayDecideSoundOnce()
    {
        if (hasPlayedDecide) return;
        hasPlayedDecide = true;

        // If an AudioSource has been provided, prefer using it.
        if (audioSource != null)
        {
            if (decideClip != null)
            {
                audioSource.PlayOneShot(decideClip);
            }
            else
            {
                // try loading from Resources/決定
                var clip = Resources.Load<AudioClip>("決定");
                if (clip != null) audioSource.PlayOneShot(clip);
            }
            return;
        }

        // If no AudioSource provided, get clip from decideClip or Resources and play via a temporary GameObject
        AudioClip clipToPlay = decideClip;
        if (clipToPlay == null)
        {
            clipToPlay = Resources.Load<AudioClip>("決定");
        }

        if (clipToPlay == null)
        {
            // nothing to play
            return;
        }

        // Create a temporary audio player that survives scene loading so the click sound isn't cut off by immediate scene change.
        GameObject go = new GameObject("DecideSoundPlayer");
        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.spatialBlend = 0f; // 2D sound
        src.PlayOneShot(clipToPlay);
        DontDestroyOnLoad(go);
        // Destroy after clip finishes
        Destroy(go, clipToPlay.length + 0.1f);
    }

    private bool IsSceneInBuild(string sceneName)
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneCount; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == sceneName) return true;
        }
        return false;
    }
}
