using UnityEngine;
using UnityEngine.SceneManagement;

public class UIScene : MonoBehaviour
{
    void Start()
    {
        // 何もしない
    }

    void Update()
    {
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
