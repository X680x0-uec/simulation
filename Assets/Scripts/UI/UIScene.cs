// ...existing code...
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIScene : MonoBehaviour
{
    // ゴール管理
    private int goalCount = 0;
    private bool resultSceneTriggered = false;

    [Header("UI SFX")]
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioClip transitionClip;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Debug.Log("[UIScene] Awake - DontDestroyOnLoad applied");
    }

    private void OnEnable()
    {
        // イベント登録（存在する場合のみ）
        try { MikoshiControllerImada.OnMikoshiReachedGoal += HandleGoal; } catch { }

        // シーン切替監視
        SceneManager.activeSceneChanged += OnSceneChanged;

        // シーンが既にTanakaならリセット
        if (SceneManager.GetActiveScene().name == "TanakaScene") ResetGameState();
    }

    private void OnDisable()
    {
        try { MikoshiControllerImada.OnMikoshiReachedGoal -= HandleGoal; } catch { }
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        Debug.Log($"[UIScene] Scene changed: {oldScene.name} -> {newScene.name}");
        if (newScene.name == "TanakaScene") ResetGameState();
    }

    private void ResetGameState()
    {
        goalCount = 0;
        resultSceneTriggered = false;
        Time.timeScale = 1f;
        Debug.Log("[UIScene] Game state reset");
    }

    private void HandleGoal()
    {
        goalCount++;
        Debug.Log($"[UIScene] HandleGoal called - count: {goalCount}");

        if (goalCount >= 3)
        {
            if (resultSceneTriggered) return;
            resultSceneTriggered = true;
            Time.timeScale = 1f;
            TryLoadScene("ResultScene");
            return;
        }

        // 1回目・2回目はゲームを停止して UI を表示する想定
        Time.timeScale = 0f;
        Debug.Log("[UIScene] Paused for ChoiceUI (goal < 3)");
    }

    void Update()
    {
        string current = SceneManager.GetActiveScene().name;

        if (current == "ExplainScene0" && Input.GetKeyDown(KeyCode.D)) { TryLoadScene("ExplainScene1"); return; }
        if (current == "ExplainScene1")
        {
            if (Input.GetKeyDown(KeyCode.A)) TryLoadScene("ExplainScene0");
            else if (Input.GetKeyDown(KeyCode.D)) TryLoadScene("ExplainScene2");
            return;
        }
        if (current == "ExplainScene2")
        {
            if (Input.GetKeyDown(KeyCode.A)) TryLoadScene("ExplainScene1");
            else if (Input.GetKeyDown(KeyCode.D)) TryLoadScene("ExplainScene3");
            return;
        }
        if (current == "ExplainScene3")
        {
            if (Input.GetKeyDown(KeyCode.A)) TryLoadScene("ExplainScene2");
            else if (Input.GetKeyDown(KeyCode.D)) TryLoadScene("ExplainScene4");
            return;
        }
        if (current == "ExplainScene4")
        {
            if (Input.GetKeyDown(KeyCode.A)) TryLoadScene("ExplainScene3");
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) TryLoadScene("TanakaScene");
            return;
        }
    }

    public void TryLoadScene(string sceneName)
    {
        if (!IsSceneInBuild(sceneName))
        {
            Debug.LogError($"[UIScene] Scene '{sceneName}' is not in Build Settings");
            return;
        }

        string current = SceneManager.GetActiveScene().name;

        bool IsExplainScene(string n) =>
            n == "ExplainScene0" || n == "ExplainScene1" || n == "ExplainScene2" || n == "ExplainScene3" || n == "ExplainScene4";

        bool ShouldPlayBetween(string cur, string tgt)
        {
            if (IsExplainScene(cur) && IsExplainScene(tgt)) return true;
            if (cur == "TanakaScene" && tgt == "ResultScene") return true;
            if (cur == "ResultScene" && tgt == "TitleScene") return true;
            return false;
        }

        if (transitionClip != null && ShouldPlayBetween(current, sceneName))
        {
            if (uiAudioSource != null) uiAudioSource.PlayOneShot(transitionClip);
            else
            {
                var tmp = new GameObject("_TempUITransitionSfx");
                var src = tmp.AddComponent<AudioSource>();
                src.clip = transitionClip;
                src.playOnAwake = false;
                src.spatialBlend = 0f;
                src.volume = AudioListener.volume;
                DontDestroyOnLoad(tmp);
                src.Play();
                Destroy(tmp, transitionClip.length + 0.1f);
            }
        }

        SceneManager.LoadScene(sceneName);
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
// ...existing code...