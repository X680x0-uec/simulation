using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIScene : MonoBehaviour
{
    // Mikoshi 到達検出用
    private MikoshiController mikoshiInstance;
    private bool resultSceneTriggered = false;
    [Header("UI SFX")]
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioClip transitionClip;
    [Header("Mikoshi UI")]
    [SerializeField] private TextMeshProUGUI mikoshiHpText;
    void Start()
    {
        // Inspector に未設定の場合はランタイムで自動生成する
        EnsureMikoshiHpTextExists();
    }

    private void EnsureMikoshiHpTextExists()
    {
        if (mikoshiHpText != null) return;

        // 既にシーン内に同名の TextMeshProUGUI があればそれを使う
        var existing = GameObject.Find("MikoshiHpText_Runtime");
        if (existing != null)
        {
            mikoshiHpText = existing.GetComponent<TextMeshProUGUI>();
            if (mikoshiHpText != null) return;
        }

    // Canvas を探すか作る（Unity バージョン差分を吸収）
    Canvas canvas = null;
#if UNITY_2023_2_OR_NEWER
    canvas = UnityEngine.Object.FindAnyObjectByType<Canvas>();
#elif UNITY_2023_1_OR_NEWER
    var _foundCanvas = UnityEngine.Object.FindObjectsByType<Canvas>(UnityEngine.FindObjectsSortMode.None);
    if (_foundCanvas != null && _foundCanvas.Length > 0) canvas = _foundCanvas[0];
#else
    canvas = UnityEngine.Object.FindObjectOfType<Canvas>();
#endif
    if (canvas == null)
    {
        var canvasGO = new GameObject("Canvas_Runtime");
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        DontDestroyOnLoad(canvasGO);
    }

        // TextMeshProUGUI を作成
        var go = new GameObject("MikoshiHpText_Runtime");
        go.transform.SetParent(canvas.transform, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = "HP:--/--";
        tmp.fontSize = 24;
        tmp.color = Color.white;
        tmp.raycastTarget = false;
        tmp.alignment = TextAlignmentOptions.TopLeft;

        // RectTransform をトップ左に固定
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(10f, -10f);
        rt.sizeDelta = new Vector2(300f, 50f);

        mikoshiHpText = tmp;
    }

    void Update()
    {
        // Mikoshi が終点に到達したら ResultScene に遷移（1回だけ）
        if (!resultSceneTriggered)
        {
            if (mikoshiInstance == null)
            {
                // Use Unity version-appropriate APIs to avoid deprecated calls
#if UNITY_2023_2_OR_NEWER
                mikoshiInstance = UnityEngine.Object.FindAnyObjectByType<MikoshiController>();
#elif UNITY_2023_1_OR_NEWER
                var found = UnityEngine.Object.FindObjectsByType<MikoshiControllerImada>(UnityEngine.FindObjectsSortMode.None);
                if (found != null && found.Length > 0) mikoshiInstance = found[0];
#else
                var found = UnityEngine.Object.FindObjectsOfType<MikoshiControllerImada>();
                if (found != null && found.Length > 0) mikoshiInstance = found[0];
#endif
            }
            if (mikoshiInstance != null && mikoshiInstance.hasReachedEnd)
            {
                resultSceneTriggered = true;
                TryLoadScene("ResultScene");
                return;
            }
        }

        string current = SceneManager.GetActiveScene().name;

        // Update Mikoshi HP display (if assigned)
        if (mikoshiHpText != null)
        {
            if (mikoshiInstance == null)
            {
#if UNITY_2023_2_OR_NEWER
                mikoshiInstance = UnityEngine.Object.FindAnyObjectByType<MikoshiControllerImada>();
#elif UNITY_2023_1_OR_NEWER
                var found2 = UnityEngine.Object.FindObjectsByType<MikoshiControllerImada>(UnityEngine.FindObjectsSortMode.None);
                if (found2 != null && found2.Length > 0) mikoshiInstance = found2[0];
#else
                var found2 = UnityEngine.Object.FindObjectsOfType<MikoshiControllerImada>();
                if (found2 != null && found2.Length > 0) mikoshiInstance = found2[0];
#endif
            }

            if (mikoshiInstance != null)
            {
                try
                {
                    mikoshiHpText.text = $"HP:{mikoshiInstance.CurrentHP}/{mikoshiInstance.MaxHP}";
                }
                catch { mikoshiHpText.text = "HP:--/--"; }
            }
            else
            {
                mikoshiHpText.text = "HP:--/--";
            }
        }

        // ResultScene handling is managed elsewhere; no per-frame Enter handling here.

        // ExplainScene1 -> D -> ExplainScene2
        if (current == "ExplainScene1" && Input.GetKeyDown(KeyCode.D))
        {
            TryLoadScene("ExplainScene2");
            return;
        }

        // ExplainScene2 -> A -> ExplainScene1
        // ExplainScene2 -> D -> ExplainScene3
        if (current == "ExplainScene2")
        {
            if (Input.GetKeyDown(KeyCode.A)) TryLoadScene("ExplainScene1");
            else if (Input.GetKeyDown(KeyCode.D)) TryLoadScene("ExplainScene3");
            return;
        }

        // ExplainScene3 -> A -> ExplainScene2
        // ExplainScene3 -> D -> ExplainScene4
        if (current == "ExplainScene3")
        {
            if (Input.GetKeyDown(KeyCode.A)) TryLoadScene("ExplainScene2");
            else if (Input.GetKeyDown(KeyCode.D)) TryLoadScene("ExplainScene4");
            return;
        }

        // ExplainScene4 -> A -> ExplainScene3
        // ExplainScene4 -> Enter -> TanakaScene
        if (current == "ExplainScene4")
        {
            if (Input.GetKeyDown(KeyCode.A)) TryLoadScene("ExplainScene3");
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) TryLoadScene("TanakaScene");
            return;
        }
    }

    public void TryLoadScene(string sceneName)
    {
        if (IsSceneInBuild(sceneName))
        {
            // Play UI transition SFX only for these flows:
            // - ExplainScene1..4 <-> ExplainScene1..4
            // - TanakaScene -> ResultScene
            // - ResultScene -> TitleScene
            bool IsExplainScene(string n)
            {
                return n == "ExplainScene1" || n == "ExplainScene2" || n == "ExplainScene3" || n == "ExplainScene4";
            }

            string current = SceneManager.GetActiveScene().name;

            bool ShouldPlayBetween(string cur, string tgt)
            {
                if (IsExplainScene(cur) && IsExplainScene(tgt)) return true;
                if (cur == "TanakaScene" && tgt == "ResultScene") return true;
                if (cur == "ResultScene" && tgt == "TitleScene") return true;
                return false;
            }

            if (transitionClip != null && ShouldPlayBetween(current, sceneName))
            {
                if (uiAudioSource != null)
                {
                    uiAudioSource.PlayOneShot(transitionClip);
                }
                else
                {
                    var tmp = new GameObject("_TempUITransitionSfx");
                    var src = tmp.AddComponent<AudioSource>();
                    src.clip = transitionClip;
                    src.playOnAwake = false;
                    src.spatialBlend = 0f;
                    src.loop = false;
                    // Respect current master volume
                    src.volume = AudioListener.volume;
                    DontDestroyOnLoad(tmp);
                    src.Play();
                    Destroy(tmp, transitionClip.length + 0.1f);
                }
            }

            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"Scene '{sceneName}' is not registered in Build Settings. Add it to Build Settings before loading.");
        }
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
