using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
public class GamepadUIScene : MonoBehaviour
{
    private InputAction enterAction;
    private InputAction backAction;
    private int sceneNumber = 1;
    void Awake()
    {
        enterAction = InputSystem.actions.FindAction("Enter");
        enterAction.performed += ctx => NextScene();
        backAction = InputSystem.actions.FindAction("Back");
        backAction.performed += ctx => PreviousScene();
    }
    void NextScene()
    {
        if ( sceneNumber == 1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("ExplainScene1");
            sceneNumber++;
        }
        else if (sceneNumber == 2)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("ExplainScene2");
            sceneNumber++;
        }
        else if (sceneNumber == 3)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("ExplainScene3");
            sceneNumber++;
        }
        else if (sceneNumber == 4)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("TanakaScene");
            sceneNumber++;
        }
    }
    void PreviousScene()
    {
        if ( sceneNumber == 1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
        }
        else if ( sceneNumber == 2)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("ExplainScene1");
            sceneNumber--;
        }
        else if (sceneNumber == 3)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("ExplainScene2");
            sceneNumber--;
        }
        else if (sceneNumber == 4)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("ExplainScene3");
            sceneNumber--;
        }
    }
}