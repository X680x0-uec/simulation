using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class UIScene : MonoBehaviour
{
    public static UIScene Instance { get; private set; }

    // Mikoshi 到達検出用
    private MikoshiControllerImada mikoshiInstance;
    private Canvas hpCanvas; // HP専用Canvas

    private int goalCount = 0;
    private bool resultSceneTriggered = false;

    [Header("UI SFX")]
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioClip transitionClip;

    [Header("Mikoshi UI")]
    [SerializeField] private TextMeshProUGUI mikoshiHpText;

    void Awake()
    {
        // シングルトン化して DontDestroyOnLoad（複製を防ぐ）
        if (Instance != null && Instance != this)
        {
            Debug.Log("[UIScene] Duplicate detected, destroying this instance.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("[UIScene] Instance set and DontDestroyOnLoad applied");

        // 起動時に EventSystem の重複がないか整理
        RemoveDuplicateEventSystems();
    }

    private void OnEnable()
    {
        Debug.Log("[UIScene] OnEnable called");
        MikoshiControllerImada.OnMikoshiReachedGoal += HandleGoal;

        // TanakaScene にロードされたときにリセット
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"[UIScene] Current Scene: {currentScene}");
        if (currentScene == "TanakaScene")
        {
            ResetGameState();
        }

        // シーン切り替え時に HP 表示を更新
        SceneManager.activeSceneChanged += OnSceneChanged;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        // 初回の表示状態を整える
        UpdateHPVisibility();
    }

    private void OnDisable()
    {
        MikoshiControllerImada.OnMikoshiReachedGoal -= HandleGoal;
        SceneManager.activeSceneChanged -= OnSceneChanged;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        Debug.Log($"[UIScene] Scene changed: {oldScene.name} -> {newScene.name}");

        // シーン変更時に mikoshiInstance と mikoshiHpText をリセット
        mikoshiInstance = null;
        mikoshiHpText = null;

        // HP表示の可視性を更新
        UpdateHPVisibility();

        // EventSystem の重複を再チェック
        RemoveDuplicateEventSystems();

        // TanakaScene に入ったら念のためリセット
        if (newScene.name == "TanakaScene")
        {
            ResetGameState();
        }
    }

    private void OnSceneUnloaded(Scene unloadedScene)
    {
        Debug.Log($"[UIScene] Scene unloaded: {unloadedScene.name}");
        RemoveDuplicateEventSystems();
    }

    // EventSystem の重複を検出して 1 つにまとめる
    private void RemoveDuplicateEventSystems()
    {
        EventSystem[] systems = GetAllEventSystems();
        if (systems == null || systems.Length <= 1) return;

        Debug.Log($"[UIScene] Found {systems.Length} EventSystems. Cleaning up duplicates.");

        EventSystem keep = EventSystem.current;

        if (keep == null)
        {
            foreach (var s in systems)
            {
                if (s.gameObject.transform.IsChildOf(transform))
                {
                    keep = s;
                    break;
                }
            }
        }

        if (keep == null) keep = systems[0];

        foreach (var s in systems)
        {
            if (s == keep) continue;
            Debug.Log($"[UIScene] Destroying duplicate EventSystem on GameObject: {s.gameObject.name}");
            Destroy(s.gameObject);
            // もし GameObject を残したい場合は Destroy(s) に変更してください
        }
    }


    // HP 表示管理 --------------------------------------------------------

    private void UpdateHPVisibility()
    {
        Debug.Log("[UIScene] UpdateHPVisibility called");

        // シーン内の既存HPテキストを探す
        FindExistingHPText();

        if (mikoshiHpText == null)
        {
            Debug.Log("[UIScene] mikoshiHpText is null, calling EnsureMikoshiHpTextExists");
            EnsureMikoshiHpTextExists();
            if (mikoshiHpText == null)
            {
                Debug.LogError("[UIScene] Failed to create mikoshiHpText!");
                return;
            }
        }

        string currentScene = SceneManager.GetActiveScene().name;

        // TanakaScene のみ表示、それ以外は非表示
        bool shouldShow = (currentScene == "TanakaScene");

        // Canvas を確認してアクティブ化
        if (hpCanvas != null)
        {
            hpCanvas.gameObject.SetActive(shouldShow);
            Debug.Log($"[UIScene] HP Canvas active: {hpCanvas.gameObject.activeSelf}");
        }

        mikoshiHpText.gameObject.SetActive(shouldShow);

        Debug.Log($"[UIScene] HP表示: {(shouldShow ? "ON" : "OFF")} (Scene: {currentScene})");
    }

    private void FindExistingHPText()
    {
        // シーン内の "MikoshiHP" という名前のオブジェクトを探す
        GameObject existingHP = GameObject.Find("MikoshiHP");
        if (existingHP != null)
        {
            var allTexts = GetAllTMPTexts();
            if (allTexts != null)
            {
                foreach (var text in allTexts)
                {
                    if (string.IsNullOrEmpty(text.text)) continue;
                    if (text.text.Contains("HP:") || text.name.Contains("HP") || text.name.Contains("Mikoshi"))
                    {
                        if (text.name.Contains("Runtime")) continue;

                        mikoshiHpText = text;
                        hpCanvas = text.GetComponentInParent<Canvas>();
                        Debug.Log($"[UIScene] シーン内のHP表示を発見して使用: {text.name}");
                        return;
                    }
                }
            }
        }
    }
    // UIScene クラス内に追加してください

    // TextMeshProUGUI をシーン内（非アクティブ含む）から取得する安全なヘルパー
    private TextMeshProUGUI[] GetAllTMPTexts()
    {
        var all = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        if (all == null || all.Length == 0) return new TextMeshProUGUI[0];

        var list = new System.Collections.Generic.List<TextMeshProUGUI>(all.Length);
        foreach (var t in all)
        {
            if (t == null) continue;
            var go = t.gameObject;
            if (go == null) continue;
            // シーンに属していて読み込まれているものだけを残す
            if (go.scene.IsValid() && go.scene.isLoaded)
            {
                list.Add(t);
            }
        }
        return list.ToArray();
    }

    // EventSystem をシーン内（非アクティブ含む）から取得する安全なヘルパー
    private EventSystem[] GetAllEventSystems()
    {
        var all = Resources.FindObjectsOfTypeAll<EventSystem>();
        if (all == null || all.Length == 0) return new EventSystem[0];

        var list = new System.Collections.Generic.List<EventSystem>(all.Length);
        foreach (var e in all)
        {
            if (e == null) continue;
            var go = e.gameObject;
            if (go == null) continue;
            if (go.scene.IsValid() && go.scene.isLoaded)
            {
                list.Add(e);
            }
        }
        return list.ToArray();
    }



    private void EnsureMikoshiHpTextExists()
    {
        Debug.Log("[UIScene] EnsureMikoshiHpTextExists called");

        if (mikoshiHpText != null)
        {
            Debug.Log("[UIScene] mikoshiHpText already exists");
            return;
        }

        // 既存の HP Canvas を探す（Runtime作成のもの）
        GameObject existingCanvas = GameObject.Find("HP_Canvas_Runtime");
        if (existingCanvas != null)
        {
            hpCanvas = existingCanvas.GetComponent<Canvas>();
            mikoshiHpText = existingCanvas.GetComponentInChildren<TextMeshProUGUI>();
            if (mikoshiHpText != null)
            {
                Debug.Log("[UIScene] 既存の HP_Canvas_Runtime を使用");
                return;
            }
        }

        // HP専用の Canvas を作成
        Debug.Log("[UIScene] Creating new HP_Canvas_Runtime");
        var canvasGO = new GameObject("HP_Canvas_Runtime");
        hpCanvas = canvasGO.AddComponent<Canvas>();
        hpCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        hpCanvas.sortingOrder = 9999; // 最前面に表示

        var scaler = canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        DontDestroyOnLoad(canvasGO);

        // TextMeshProUGUI を作成
        var go = new GameObject("MikoshiHpText_Runtime");
        go.transform.SetParent(canvasGO.transform, false);

        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = "HP: 100/100";
        tmp.fontSize = 48;
        tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;
        tmp.raycastTarget = false;
        tmp.alignment = TextAlignmentOptions.TopLeft;

        // 影をつけて見やすく
        var outline = go.AddComponent<UnityEngine.UI.Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(3, -3);

        // RectTransform を画面左上に固定
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(30f, -30f);
        rt.sizeDelta = new Vector2(500f, 80f);

        mikoshiHpText = tmp;
        Debug.Log($"[UIScene] HP表示を作成完了 (Position: {rt.anchoredPosition}, Size: {rt.sizeDelta})");
    }

    void Start()
    {
        Debug.Log("[UIScene] Start called");

        // シーン内の既存HPテキストを探す
        FindExistingHPText();

        // Inspector に未設定の場合はランタイムで自動生成する
        if (mikoshiHpText == null)
        {
            EnsureMikoshiHpTextExists();
        }

        // 初回の表示状態を設定
        UpdateHPVisibility();
    }

    void Update()
    {
        string current = SceneManager.GetActiveScene().name;

        // TanakaScene でのみ HP を更新
        if (current == "TanakaScene" && mikoshiHpText != null && mikoshiHpText.gameObject.activeSelf)
        {
            // mikoshiInstance のキャッシュ
            if (mikoshiInstance == null)
            {
#if UNITY_2023_2_OR_NEWER
                mikoshiInstance = UnityEngine.Object.FindAnyObjectByType<MikoshiControllerImada>();
#elif UNITY_2023_1_OR_NEWER
                var found = UnityEngine.Object.FindObjectsByType<MikoshiControllerImada>(UnityEngine.FindObjectsSortMode.None);
                if (found != null && found.Length > 0) mikoshiInstance = found[0];
#else
                var found = UnityEngine.Object.FindObjectsOfType<MikoshiControllerImada>();
                if (found != null && found.Length > 0) mikoshiInstance = found[0];
#endif
            }

            // HP 表示を更新（現在HP/最大HP）
            if (mikoshiInstance != null)
            {
                try
                {
                    string hpText = $"HP: {mikoshiInstance.CurrentHP}/{mikoshiInstance.MaxHP}";
                    if (mikoshiHpText.text != hpText)
                    {
                        mikoshiHpText.text = hpText;
                    }
                }
                catch
                {
                    mikoshiHpText.text = "HP: --/--";
                }
            }
            else
            {
                mikoshiHpText.text = "HP: --/--";
            }
        }

        // ExplainScene のキー遷移（既存ロジック維持）
        if (current == "ExplainScene0" && Input.GetKeyDown(KeyCode.D))
        {
            TryLoadScene("ExplainScene1");
            return;
        }
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

    private void ResetGameState()
    {
        goalCount = 0;
        resultSceneTriggered = false;
        Time.timeScale = 1f;
        mikoshiInstance = null;
        Debug.Log("[UIScene] ゲーム状態をリセットしました");
    }

    private void HandleGoal()
    {
        // ゴール回数を増やす
        goalCount++;
        Debug.Log($"[UIScene] ゴール回数：{goalCount}");

        // --- ３回目のゴールで ResultScene へ ---
        if (goalCount >= 3)
        {
            if (resultSceneTriggered) return;
            resultSceneTriggered = true;

            Time.timeScale = 1f;
            TryLoadScene("ResultScene");
            return;
        }

        // --- 1回目・2回目のゴール → いつも通り ChoiceUI を表示 ---
        Debug.Log("[UIScene] ChoiceUI を表示します（1 or 2 回目）");

        // GameFlowManager に通知されて ChoiceUI が表示される想定
        Time.timeScale = 0f;
    }

    public void TryLoadScene(string sceneName)
    {
        if (IsSceneInBuild(sceneName))
        {
            bool IsExplainScene(string n)
            {
                return n == "ExplainScene0" || n == "ExplainScene1" || n == "ExplainScene2" || n == "ExplainScene3" || n == "ExplainScene4";
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