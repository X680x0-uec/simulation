using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceUI : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonParent;

    private GameFlowManager manager;

    // 1引数版
    public void ShowChoices(GameFlowManager flowManager)
    {
        manager = flowManager;
        gameObject.SetActive(true);

        // 古いボタンを削除
        foreach (Transform child in buttonParent)
            Destroy(child.gameObject);

        // 例: 2～3個のボタンを生成
        int choiceCount = 2; // 必要に応じて増やす
        for (int i = 0; i < choiceCount; i++)
        {
            int choiceIndex = i;
            GameObject btnObj = Instantiate(buttonPrefab, buttonParent);

            TMP_Text text = btnObj.GetComponentInChildren<TMP_Text>();
            if (text != null)
                text.text = $" {i + 1}";

            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(() => manager.OnChoiceSelected(choiceIndex));
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
