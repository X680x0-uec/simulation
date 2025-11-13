using UnityEngine;
using UnityEngine.SceneManagement;

public class ExplainSceneBack : MonoBehaviour
{
    [SerializeField] private string returnScene = "TitleScene";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsSceneInBuild(returnScene))
            {
                SceneManager.LoadScene(returnScene);
            }
            else
            {
                Debug.LogError($"Scene '{returnScene}' is not registered in Build Settings. Add it before trying to load.");
            }
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
