using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ChoiceUI : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonParent;
    [SerializeField] private float buttonTextSize = 36f;
    [SerializeField] private Vector2 buttonSize = new Vector2(200f, 80f);
    [SerializeField] private VirtualMousePositionAdjuster virtualMousePositionAdjuster;

    private Action<int> onChoiceCallback;

    public void ShowChoices(string[] options, Action<int> callback)
    {
        onChoiceCallback = callback;
        gameObject.SetActive(true);
        virtualMousePositionAdjuster.isActive = true;

        // 古いボタンを削除
        foreach (Transform child in buttonParent)
            Destroy(child.gameObject);

        // ボタン生成
        for (int i = 0; i < options.Length; i++)
        {
            int choiceIndex = i;
            GameObject btnObj = Instantiate(buttonPrefab, buttonParent);

            // ボタンサイズ設定（LayoutGroupが制御するのでsizeDeltaのみでOK）
            RectTransform rectTransform = btnObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = buttonSize;
            }

            // テキスト設定
            TMP_Text text = btnObj.GetComponentInChildren<TMP_Text>();
            if (text != null)
            {
                text.text = options[i];
                text.fontSize = buttonTextSize;
            }

            // クリックイベント
            Button btn = btnObj.GetComponent<Button>();
            // Debug.Log("ボタンは押された。番号は"+ options[i]);
            if (btn != null)
            {
                Debug.Log($"[ChoiceUI] Setting up button for option: {options[i]}");
                var clickedEvent = new Button.ButtonClickedEvent();
                clickedEvent.AddListener(() => OnButtonClicked(choiceIndex));
                btn.onClick = clickedEvent;
            }
        }
    }

    private void OnButtonClicked(int index)
    {
        onChoiceCallback?.Invoke(index);
        Debug.Log($"[ChoiceUI] Button {index} clicked.");
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        virtualMousePositionAdjuster.isActive = false;
    }
}
