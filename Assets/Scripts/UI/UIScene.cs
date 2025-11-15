using UnityEngine;
using UnityEngine.SceneManagement;

public class UIScene : MonoBehaviour
{
    // Mikoshi 到達検出用
    private MikoshiController mikoshiInstance;
    private bool resultSceneTriggered = false;
    void Start()
    {
        // 何もしない
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

    private void TryLoadScene(string sceneName)
    {
        if (IsSceneInBuild(sceneName))
        {
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
