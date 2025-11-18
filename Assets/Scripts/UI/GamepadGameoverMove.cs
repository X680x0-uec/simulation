using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GamepadGameoverMove : MonoBehaviour
{
    private InputAction GmoveAction;
    [Header("UI SFX")]
    [SerializeField] private AudioSource UIAudioSource;
    [SerializeField] private AudioClip transitionClip;

    void Awake()
    {
        GmoveAction = InputSystem.actions.FindAction("EnterResult");
        Debug.Log("GamepadGameoverMove Awake: moveAction assigned");
    }

    private void OnEnable()
    {
        // Register only if we're currently in ResultScene
        if (SceneManager.GetActiveScene().name == "GameOverScene")
        {
            if (GmoveAction != null)
            {
                GmoveAction.performed += OnMovePerformed;
                GmoveAction.Enable();
            }
            Debug.Log("GamepadGameoverMove enabled in GameOverScene");
        }
    }

    private void OnDisable()
    {
        if (GmoveAction != null)
        {
            GmoveAction.performed -= OnMovePerformed;
            GmoveAction.Disable();
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        GToTitleScene();
    }

    void GToTitleScene()
    {
        // Ensure this only runs when actually in ResultScene (defensive)
        if (SceneManager.GetActiveScene().name != "GameOverScene") return;

        Debug.Log("GamepadGameoverMove: ToTitleScene triggered");
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
