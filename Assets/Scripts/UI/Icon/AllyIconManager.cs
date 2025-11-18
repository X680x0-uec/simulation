using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class AllyIconManager : MonoBehaviour
{
    [SerializeField] private AllyManager allyManager;
    [SerializeField] private GameObject[] AllyIcons;

    private Image[] AllyIconImages;
    private TextMeshProUGUI[] AllyIconTexts;

    [Header("アイコンのサイズ設定")]
    [SerializeField] private Vector2 defaultSize = new Vector2(100f, 100f);
    [SerializeField] private Vector2 centerSize = new Vector2(150f, 150f);
    [Header("アイコンの間隔設定")]
    [SerializeField] private float iconSpacing = 120f;

    // ★追加: 選択中と非選択中の色設定
    [Header("色の設定")]
    [SerializeField] private Color selectedColor = Color.white; // 選択中（真っ白＝元の色）
    [SerializeField] private Color unselectedColor = new Color(0.5f, 0.5f, 0.5f, 1f); // 非選択（グレー）

    void Awake()
    {
        AllyIconImages = new Image[3];
        AllyIconTexts = new TextMeshProUGUI[3];

        for (int i = 0; i < AllyIcons.Length; i++)
        {
            AllyIconImages[i] = AllyIcons[i].GetComponent<Image>();
            AllyIconTexts[i] = AllyIcons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (AllyIconImages[i] != null) AllyIconImages[i].preserveAspect = true;
        }
        // ★追加: 真ん中のアイコン(index 1)を最前面（ヒエラルキーの最後）に移動させる
        if (AllyIcons.Length > 1 && AllyIcons[1] != null)
        {
            AllyIcons[1].transform.SetAsLastSibling();
        }

        SwitchIcon(AllyManager.index, allyManager.allyDatabase);
    }

    public void SwitchIcon(int newIndex, AllyDatabase allyDatabase)
    {
        for (int i = 0; i < AllyIcons.Length; i++)
        {
            if (AllyIconImages[i] != null)
            {
                int index = (newIndex - 1 + i + allyDatabase.allAllies.Count) % allyDatabase.allAllies.Count;
                AllyData ally = allyDatabase.allAllies[index];
                RectTransform rect = AllyIconImages[i].rectTransform;

                // --- 1. 画像とテキストの更新 ---
                if (ally.icon != null)
                {
                    AllyIconImages[i].sprite = ally.icon;
                    AllyIconTexts[i].text = "";

                    // ★変更: ここで色を決める
                    // i == 1 (真ん中) なら「選択色」、それ以外は「非選択色」
                    if (i == 1)
                    {
                        AllyIconImages[i].color = selectedColor;
                    }
                    else
                    {
                        AllyIconImages[i].color = unselectedColor;
                    }
                }
                else
                {
                    // 画像がない場合は透明にする
                    AllyIconImages[i].sprite = null;
                    AllyIconTexts[i].text = ally.name;
                    AllyIconImages[i].color = Color.clear;
                }

                // --- 2. サイズと位置の調整 ---
                if (i == 1) // 真ん中
                {
                    rect.sizeDelta = centerSize;
                    rect.anchoredPosition = Vector2.zero;
                }
                else if (i == 0) // 左
                {
                    rect.sizeDelta = defaultSize;
                    rect.anchoredPosition = new Vector2(-iconSpacing, 0);
                }
                else if (i == 2) // 右
                {
                    rect.sizeDelta = defaultSize;
                    rect.anchoredPosition = new Vector2(iconSpacing, 0);
                }
            }
        }
    }
}