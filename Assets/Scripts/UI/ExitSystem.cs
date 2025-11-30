using Unity.VisualScripting;
using UnityEngine;

public class ExitSystem : MonoBehaviour
{
    public void OnExitButtonClicked()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
