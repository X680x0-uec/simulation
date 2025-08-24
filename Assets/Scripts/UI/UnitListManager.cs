using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using TMPro;

public class UnitListManager : MonoBehaviour
{
    [SerializeField] private float margin = 5f;
    [SerializeField] private GameObject unitIconPrefab;
    private float iconSize;
    [SerializeField] private RectTransform contentPanel;

    void Start()
    {
        iconSize = unitIconPrefab.GetComponent<RectTransform>().sizeDelta.x;
        List<UnitData> units = PlayerUnitManager.unlockedUnits;
        RefreshUnitList();
    }

    void RefreshUnitList()
    {
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        contentPanel.sizeDelta = new Vector2(contentPanel.sizeDelta.x, 90f * (PlayerUnitManager.unlockedUnits.Count / 3 + 1));

        for (int i = 0; i < PlayerUnitManager.unlockedUnits.Count; i++)
        {
            UnitData unit = PlayerUnitManager.unlockedUnits[i];
            GameObject icon = Instantiate(unitIconPrefab, contentPanel);
            Image iconImage = icon.GetComponent<Image>();
            Button iconButton = icon.GetComponent<Button>();
            TextMeshProUGUI iconText = icon.GetComponentInChildren<TextMeshProUGUI>();
            RectTransform iconTransform = icon.GetComponent<RectTransform>();

            int row = Math.DivRem(i, 3, out int column);
            iconTransform.anchoredPosition = new Vector2(column * (iconSize + margin) + margin, -(row * (iconSize + margin) + margin));

            if (unit.icon != null)
            {
                iconImage.sprite = unit.icon;
                Destroy(iconText.gameObject); // テキストを削除
            }
            else
            {
                iconText.text = unit.unitName;
            }

            iconButton.onClick.AddListener(() => OnUnitIconClicked(unit));
        }
    }

    void OnUnitIconClicked(UnitData unit)
    {
        Debug.Log(unit.unitName + " icon clicked!");
    }
}
