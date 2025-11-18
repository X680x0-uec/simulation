using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GamepadResultMove : MonoBehaviour
{
    private InputAction moveAction;
    [Header("UI SFX")]
    [SerializeField] private AudioSource UIAudioSource;
    [SerializeField] private AudioClip transitionClip;

    void Awake()
    {
        moveAction = InputSystem.actions.FindAction("EnterResult");
        Debug.Log("GamepadResultMove Awake: moveAction assigned");
    }

    private void OnEnable()
    {
        // Register only if we're currently in ResultScene
        if (SceneManager.GetActiveScene().name == "ResultScene")
        {
            if (moveAction != null)
            {
                moveAction.performed += OnMovePerformed;
                moveAction.Enable();
            }
            Debug.Log("GamepadResultMove enabled in ResultScene");
        }
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.performed -= OnMovePerformed;
            moveAction.Disable();
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        ToTitleScene();
    }

    void ToTitleScene()
    {
        // Ensure this only runs when actually in ResultScene (defensive)
        if (SceneManager.GetActiveScene().name != "ResultScene") return;

        Debug.Log("GamepadResultMove: ToTitleScene triggered");
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
