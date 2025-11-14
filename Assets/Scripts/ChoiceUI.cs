using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ChoiceUI : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonParent;

    private Action<int> onChoiceCallback;

    // 新しい版：string配列と選択時のコールバックを受け取る
    public void ShowChoices(string[] options, Action<int> callback)
    {
        onChoiceCallback = callback;
        gameObject.SetActive(true);

        // 古いボタンを削除
        foreach (Transform child in buttonParent)
            Destroy(child.gameObject);

        // options の数だけボタンを生成
        for (int i = 0; i < options.Length; i++)
        {
            int choiceIndex = i;
            GameObject btnObj = Instantiate(buttonPrefab, buttonParent);

            TMP_Text text = btnObj.GetComponentInChildren<TMP_Text>();
            if (text != null)
                text.text = options[i];  // options[i] を使用

            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(() => OnButtonClicked(choiceIndex));
        }
    }

    private void OnButtonClicked(int index)
    {
        onChoiceCallback?.Invoke(index);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}