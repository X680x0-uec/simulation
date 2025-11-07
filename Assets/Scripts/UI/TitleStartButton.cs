using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleStartButton : MonoBehaviour
{
    [SerializeField] private string targetScene = "ExplainScene1";

    public void OnStartButtonClicked()
    {
        if (IsSceneInBuild(targetScene))
        {
            SceneManager.LoadScene(targetScene);
        }
        else
        {
            Debug.LogError($"Scene '{targetScene}' is not registered in Build Settings. Add it before trying to load.");
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
