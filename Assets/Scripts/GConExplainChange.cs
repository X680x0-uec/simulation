using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GConExplainChange : MonoBehaviour
{
    private InputAction GnextAction;
    [Header("UI SFX")]
    [SerializeField] private AudioSource UIAudioSource;
    [SerializeField] private AudioClip transitionClip;

    void Awake()
    {
        GnextAction = InputSystem.actions.FindAction("EnterResult");
        Debug.Log("GConExplainChange Awake: moveAction assigned");
    }

    private void OnEnable()
    {
        // Register only if we're currently in ResultScene
        if (SceneManager.GetActiveScene().name == "ExplainScene0" ||
            SceneManager.GetActiveScene().name == "ExplainScene1" ||
            SceneManager.GetActiveScene().name == "ExplainScene2" ||
            SceneManager.GetActiveScene().name == "ExplainScene3"||
            SceneManager.GetActiveScene().name == "ExplainScene4")
        {
            if (GnextAction != null)
            {
                GnextAction.performed += OnMovePerformed;
                GnextAction.Enable();
            }
            Debug.Log("GConExplainChange enabled in ExplainScene");
        }
    }

    private void OnDisable()
    {
        if (GnextAction != null)
        {
            GnextAction.performed -= OnMovePerformed;
            GnextAction.Disable();
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        GToNextScene1();
        GToNextScene2();
        GToNextScene3();
        GToNextScene4();
        GToTanakaScene();
    }

    void GToNextScene1()
    {
        // Ensure this only runs when actually in ResultScene (defensive)
        if (SceneManager.GetActiveScene().name != "ExplainScene0") return;

        Debug.Log("GConExplainChange: ToNextScene triggered");
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

    void GToNextScene2()
    {
        // Ensure this only runs when actually in ResultScene (defensive)
        if (SceneManager.GetActiveScene().name != "ExplainScene1") return;

        Debug.Log("GConExplainChange: ToNextScene triggered");
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

    void GToNextScene3()
    {
        // Ensure this only runs when actually in ResultScene (defensive)
        if (SceneManager.GetActiveScene().name != "ExplainScene2") return;

        Debug.Log("GConExplainChange: ToNextScene triggered");
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

    void GToNextScene4()
    {
        // Ensure this only runs when actually in ResultScene (defensive)
        if (SceneManager.GetActiveScene().name != "ExplainScene3") return;

        Debug.Log("GConExplainChange: ToNextScene triggered");
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

        SceneManager.LoadScene("ExplainScene4");
    }

    void GToTanakaScene()
    {
        // Ensure this only runs when actually in ResultScene (defensive)
        if (SceneManager.GetActiveScene().name != "ExplainScene4") return;

        Debug.Log("GConExplainChange: ToNextScene triggered");
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

        SceneManager.LoadScene("TanakaScene");
    }
}
