using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using JetBrains.Annotations;

public class ResultControler : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI resultText;
    [Header("UI SFX")]
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioClip transitionClip;

    void Start()
    {
        if (resultText == null)
        {
            Debug.LogWarning("ResultControler: resultText is not assigned in the Inspector.");
        }
        else
        {
            // Configure TMP so the text auto-sizes uniformly and wraps normally to avoid clipping.
            var t = resultText.GetType();

            // 1) Enable wrapping (prefer textWrappingMode=Wrap, fallback to enableWordWrapping=true)
            var twProp = t.GetProperty("textWrappingMode");
            if (twProp != null)
            {
                var enumType = twProp.PropertyType;
                // choose an enum value that indicates wrapping (names vary across versions)
                var wrapName = System.Enum.GetNames(enumType).FirstOrDefault(n => n.ToLower().Contains("wrap") && !n.ToLower().Contains("no"));
                if (wrapName == null) wrapName = System.Enum.GetNames(enumType).FirstOrDefault();
                if (wrapName != null)
                {
                    var enumValue = System.Enum.Parse(enumType, wrapName);
                    twProp.SetValue(resultText, enumValue);
                }
            }
            else
            {
                var ewProp = t.GetProperty("enableWordWrapping");
                if (ewProp != null) ewProp.SetValue(resultText, true);
            }

            // 2) Enable auto-sizing (uniform scaling for the whole text block)
            var autoProp = t.GetProperty("enableAutoSizing") ?? t.GetProperty("enableAutoSize");
            if (autoProp != null && autoProp.PropertyType == typeof(bool))
            {
                autoProp.SetValue(resultText, true);
            }

            // 3) Compute a larger fontSizeMax so text can be large (approx half-screen height across lines),
            //    and set a reasonable minimum so it doesn't shrink too small.
            var fontSizeProp = t.GetProperty("fontSize");
            float currentSize = 36f;
            if (fontSizeProp != null)
            {
                var val = fontSizeProp.GetValue(resultText);
                if (val is float f) currentSize = f;
                else if (val is int i) currentSize = i;
            }

            // Determine a target max font size based on screen height: aim for roughly half the screen height split across lines
            int lineCount = 3;
            float screenBasedMax = (Screen.height * 0.5f) / lineCount; // pixels per line
            // Clamp sensible range
            float desiredMax = Mathf.Clamp(screenBasedMax, 24f, 96f);

            var fontMaxProp = t.GetProperty("fontSizeMax");
            var fontMinProp = t.GetProperty("fontSizeMin");
            if (fontMaxProp != null)
            {
                fontMaxProp.SetValue(resultText, desiredMax);
            }
            if (fontMinProp != null)
            {
                // Ensure minimum is not too small so text remains readable
                fontMinProp.SetValue(resultText, Mathf.Max(14f, desiredMax * 0.35f));
            }

            // 4) Ensure overflow mode DOES NOT use Ellipsis/Truncate; prefer Overflow so text is never abbreviated.
            var omProp = t.GetProperty("overflowMode");
            if (omProp != null)
            {
                var enumType2 = omProp.PropertyType;
                // prefer an 'Overflow' enum name if present
                var preferredOverflow = System.Enum.GetNames(enumType2).FirstOrDefault(n => n.ToLower().Contains("overflow"))
                                        ?? System.Enum.GetNames(enumType2).FirstOrDefault();
                if (preferredOverflow != null)
                {
                    var enumValue2 = System.Enum.Parse(enumType2, preferredOverflow);
                    omProp.SetValue(resultText, enumValue2);
                }
            }

            // Position the result text at the center of the screen and size it
            try
            {
                var rt = resultText.rectTransform;
                // center anchors
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = Vector2.zero;

                // size: ~80% width and 50% height of the screen (in pixels)
                float targetWidth = Screen.width * 0.8f;
                float targetHeight = Screen.height * 0.5f;
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetWidth);
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetHeight);

                // center-align the text
                resultText.alignment = TMPro.TextAlignmentOptions.Center;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("ResultControler: failed to center resultText - " + ex.Message);
            }
        }
    }

    void Update()
    {
        if (resultText == null) return;
        int spawnA = 0, spawnB = 0, spawnC = 0;
        if (AllyManager.NumSpawn != null)
        {
            if (AllyManager.NumSpawn.Length > 0) spawnA = AllyManager.NumSpawn[0];
            if (AllyManager.NumSpawn.Length > 1) spawnB = AllyManager.NumSpawn[1];
            if (AllyManager.NumSpawn.Length > 2) spawnC = AllyManager.NumSpawn[2];
        }

        // Read game stats (enemy kills and ally deaths)
        int killedEnemies = 0;
        int deadA = 0, deadB = 0, deadC = 0;
        try
        {
            killedEnemies = GameStats.KilledEnemies;
            if (GameStats.AllyDeaths != null)
            {
                if (GameStats.AllyDeaths.Length > 0) deadA = GameStats.AllyDeaths[0];
                if (GameStats.AllyDeaths.Length > 1) deadB = GameStats.AllyDeaths[1];
                if (GameStats.AllyDeaths.Length > 2) deadC = GameStats.AllyDeaths[2];
            }
        }
        
        catch { }

        // スコアは『200*倒した敵の総数*召喚した味方の総数/倒れた味方の総数』で計算。

        int scorePoint = 200 * killedEnemies * (spawnA + spawnB + spawnC)/(deadA + deadB + deadC);

        // 非改行スペース(U+00A0)を使って単語の途中で改行されないようにする
        string nbsp = "\u00A0";
        string kLine = $"Killed Enemies:{nbsp}{killedEnemies}{nbsp}";
        string dLine1 = $"Killed Attackers:{nbsp}{deadA}{nbsp}";
        string dLine2 = $"Killed Defencers:{nbsp}{deadB}{nbsp}";
        string dLine3 = $"Killed Archers:{nbsp}{deadC}{nbsp}";

        string sLine1 = $"Spawned Attackers:{nbsp}{spawnB}{nbsp}";
        string sLine2 = $"Spawned Defencers:{nbsp}{spawnA}{nbsp}";
        string sLine3 = $"Spawned Archers:{nbsp}{spawnC}{nbsp}";

        string pLine = $"Total Score:{scorePoint}";

        string text = string.Join("\n", new[] { kLine, dLine1, dLine2, dLine3, "", sLine1, sLine2, sLine3, "", pLine });
        resultText.text = text;
        // Press Enter to go back to TitleScene (if present in Build Settings)
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            const string titleScene = "TitleScene";
                if (IsSceneInBuild(titleScene))
                {
                // Reset stats shown on the result screen before leaving
                try
                {
                    GameStats.ResetAll();
                }
                catch { }

                try
                {
                    if (AllyManager.NumSpawn != null)
                    {
                        for (int i = 0; i < AllyManager.NumSpawn.Length; i++) AllyManager.NumSpawn[i] = 0;
                    }
                }
                catch { }

                    // Prefer using UIScene.TryLoadScene so transition SFX logic is respected if UIScene exists.
                    UIScene uiScene = null;
#if UNITY_2023_2_OR_NEWER
                    uiScene = UnityEngine.Object.FindAnyObjectByType<UIScene>();
#elif UNITY_2023_1_OR_NEWER
                    {
                        var found = UnityEngine.Object.FindObjectsByType<UIScene>(UnityEngine.FindObjectsSortMode.None);
                        if (found != null && found.Length > 0) uiScene = found[0];
                    }
#else
                    {
                        var found = UnityEngine.Object.FindObjectsOfType<UIScene>();
                        if (found != null && found.Length > 0) uiScene = found[0];
                    }
#endif
                    if (uiScene != null)
                    {
                        uiScene.TryLoadScene(titleScene);
                    }
                    else
                    {
                        // If this Result scene doesn't have a UIScene helper, play a local transitionClip if assigned
                        if (transitionClip != null)
                        {
                            if (uiAudioSource != null)
                            {
                                uiAudioSource.PlayOneShot(transitionClip);
                            }
                            else
                            {
                                var tmp = new GameObject("_TempResultTransitionSfx");
                                var src = tmp.AddComponent<AudioSource>();
                                src.clip = transitionClip;
                                src.playOnAwake = false;
                                src.spatialBlend = 0f;
                                src.loop = false;
                                src.volume = AudioListener.volume;
                                UnityEngine.Object.DontDestroyOnLoad(tmp);
                                src.Play();
                                UnityEngine.Object.Destroy(tmp, transitionClip.length + 0.1f);
                            }
                        }

                        SceneManager.LoadScene(titleScene);
                    }
            }
            else
            {
                Debug.LogWarning($"ResultControler: scene '{titleScene}' not found in Build Settings. Cannot load.");
            }
        }
    }

    private bool IsSceneInBuild(string sceneName)
    {
        int count = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < count; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = Path.GetFileNameWithoutExtension(path);
            if (string.Equals(name, sceneName, System.StringComparison.OrdinalIgnoreCase)) return true;
        }
        return false;
    }
}
