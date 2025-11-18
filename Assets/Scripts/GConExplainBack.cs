using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GConExplainBack : MonoBehaviour
{
    private InputAction GbackAction;
    [Header("UI SFX")]
    [SerializeField] private AudioSource UIAudioSource;
    [SerializeField] private AudioClip transitionClip;

    void Awake()
    {
        GbackAction = InputSystem.actions.FindAction("Back");
        Debug.Log("GConExplainBack Awake: moveAction assigned");
    }

    private void OnEnable()
    {
        // Register only if we're currently in ResultScene
        if (SceneManager.GetActiveScene().name == "ExplainScene4" ||
            SceneManager.GetActiveScene().name == "ExplainScene3" ||
            SceneManager.GetActiveScene().name == "ExplainScene2" ||
            SceneManager.GetActiveScene().name == "ExplainScene1"||
            SceneManager.GetActiveScene().name == "ExplainScene0")
        {
            if (GbackAction != null)
            {
                GbackAction.performed += OnMovePerformed;
                GbackAction.Enable();
            }
            Debug.Log("GConExplainBack enabled in ExplainScene");
        }
    }

    private void OnDisable()
    {
        if (GbackAction != null)
        {
            GbackAction.performed -= OnMovePerformed;
            GbackAction.Disable();
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        GToTitleScene();
        GToBackScene4();
        GToBackScene3();
        GToBackScene2();
        GToBackScene1();
    }

    void GToBackScene1()
    {
        // Ensure this only runs when actually in ResultScene (defensive)
        if (SceneManager.GetActiveScene().name != "ExplainScene1") return;

        Debug.Log("GConExplainBack: ToNextScene triggered");
        // Play transition SFX similarly to ResultControler/UIScene so sound isn't cut by scene load
        if (transitionClip != null)
        {
            if (UIAudioSource != null)
            {
                UIAudioSource.PlayOneShot(transitionClip);
            }
            else
            {
                var tmp = new GameObject("_TempGamepadResultTransitionSfx");
                var src = tmp.AddComponent<AudioSource>();
                src.clip = transitionClip;
                src.playOnAwake = false;
                src.spatialBlend = 0f;
                src.loop = false;
                src.volume = AudioListener.volume;
                Object.DontDestroyOnLoad(tmp);
                src.Play();
                Object.Destroy(tmp, transitionClip.length + 0.1f);
            }
        }

        SceneManager.LoadScene("ExplainScene0");
    }

    void GToBackScene2()
    {
        // Ensure this only runs when actually in ResultScene (defensive)
        if (SceneManager.GetActiveScene().name != "ExplainScene2") return;

        Debug.Log("GConExplainBack: ToNextScene triggered");
        // Play transition SFX similarly to ResultControler/UIScene so sound isn't cut by scene load
        if (transitionClip != null)
        {
            if (UIAudioSource != null)
            {
                UIAudioSource.PlayOneShot(transitionClip);
            }
            else
            {
                var tmp = new GameObject("_TempGamepadResultTransitionSfx");
                var src = tmp.AddComponent<AudioSource>();
                src.clip = transitionClip;
                src.playOnAwake = false;
                src.spatialBlend = 0f;
                src.loop = false;
                src.volume = AudioListener.volume;
                Object.DontDestroyOnLoad(tmp);
                src.Play();
                Object.Destroy(tmp, transitionClip.length + 0.1f);
            }
        }

        SceneManager.LoadScene("ExplainScene1");
    }

    void GToBackScene3()
    {
        // Ensure this only runs when actually in ResultScene (defensive)
        if (SceneManager.GetActiveScene().name != "ExplainScene3") return;

        Debug.Log("GConExplainBack: ToNextScene triggered");
        // Play transition SFX similarly to ResultControler/UIScene so sound isn't cut by scene load
        if (transitionClip != null)
        {
            if (UIAudioSource != null)
            {
                UIAudioSource.PlayOneShot(transitionClip);
            }
            else
            {
                var tmp = new GameObject("_TempGamepadResultTransitionSfx");
                var src = tmp.AddComponent<AudioSource>();
                src.clip = transitionClip;
                src.playOnAwake = false;
                src.spatialBlend = 0f;
                src.loop = false;
                src.volume = AudioListener.volume;
                Object.DontDestroyOnLoad(tmp);
                src.Play();
                Object.Destroy(tmp, transitionClip.length + 0.1f);
            }
        }

        SceneManager.LoadScene("ExplainScene2");
    }

    void GToBackScene4()
    {
        // Ensure this only runs when actually in ResultScene (defensive)
        if (SceneManager.GetActiveScene().name != "ExplainScene4") return;

        Debug.Log("GConExplainBack: ToNextScene triggered");
        // Play transition SFX similarly to ResultControler/UIScene so sound isn't cut by scene load
        if (transitionClip != null)
        {
            if (UIAudioSource != null)
            {
                UIAudioSource.PlayOneShot(transitionClip);
            }
            else
            {
                var tmp = new GameObject("_TempGamepadResultTransitionSfx");
                var src = tmp.AddComponent<AudioSource>();
                src.clip = transitionClip;
                src.playOnAwake = false;
                src.spatialBlend = 0f;
                src.loop = false;
                src.volume = AudioListener.volume;
                Object.DontDestroyOnLoad(tmp);
                src.Play();
                Object.Destroy(tmp, transitionClip.length + 0.1f);
            }
        }

        SceneManager.LoadScene("ExplainScene3");
    }

    void GToTitleScene()
    {
        // Ensure this only runs when actually in ResultScene (defensive)
        if (SceneManager.GetActiveScene().name != "ExplainScene0") return;

        Debug.Log("GConExplainBack: ToNextScene triggered");
        // Play transition SFX similarly to ResultControler/UIScene so sound isn't cut by scene load
        if (transitionClip != null)
        {
            if (UIAudioSource != null)
            {
                UIAudioSource.PlayOneShot(transitionClip);
            }
            else
            {
                var tmp = new GameObject("_TempGamepadResultTransitionSfx");
                var src = tmp.AddComponent<AudioSource>();
                src.clip = transitionClip;
                src.playOnAwake = false;
                src.spatialBlend = 0f;
                src.loop = false;
                src.volume = AudioListener.volume;
                Object.DontDestroyOnLoad(tmp);
                src.Play();
                Object.Destroy(tmp, transitionClip.length + 0.1f);
            }
        }

        SceneManager.LoadScene("TitleScene");
    }
}
