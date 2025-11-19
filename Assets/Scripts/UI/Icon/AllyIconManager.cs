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

    // --- 修正箇所
    [Header("アイコンのサイズ設定")]
    [SerializeField] private Vector2 defaultSize = new Vector2(100f, 100f);
    [SerializeField] private Vector2 centerSize = new Vector2(150f, 150f);

    [Header("アイコンの間隔設定")]
    [SerializeField] private float iconSpacing = 120f;

    [Header("色の設定")]
    [SerializeField] private Color selectedColor = Color.white; // 選択中のアイコンの色
    [SerializeField] private Color unselectedColor = new Color(0.5f, 0.5f, 0.5f, 1f); // 未選択（グレー）
    // ---------------------------------------------------

    void OnEnable()
    {
        Debug.Log("AllyIconManagerのAwake関数が呼ばれました！！");
        AllyIconImages = new Image[3];
        AllyIconTexts = new TextMeshProUGUI[3];

        for (int i = 0; i < AllyIcons.Length; i++)
        {
            AllyIconImages[i] = AllyIcons[i].GetComponent<Image>();
            AllyIconTexts[i] = AllyIcons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (AllyIconImages[i] != null) AllyIconImages[i].preserveAspect = true;
        }
        // ���ǉ�: �^�񒆂̃A�C�R��(index 1)���őO�ʁi�q�G�����L�[�̍Ō�j�Ɉړ�������
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
            Debug.Log("AllyIconが切り替わる準備はできています。現在のAllyIconImages[i]は"+ AllyIconImages[i]);
            if (AllyIconImages[i] != null)
            {
                int index = (newIndex - 1 + i + allyDatabase.allAllies.Count) % allyDatabase.allAllies.Count;
                AllyData ally = allyDatabase.allAllies[index];
                RectTransform rect = AllyIconImages[i].rectTransform;

                // --- 1. �摜�ƃe�L�X�g�̍X�V ---
                if (ally.icon != null)
                {
                    AllyIconImages[i].sprite = ally.icon;
                    AllyIconTexts[i].text = "";

                    // ���ύX: �����ŐF�����߂�
                    // i == 1 (�^��) �Ȃ�u�I��F�v�A����ȊO�́u��I��F�v
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
                    // �摜���Ȃ��ꍇ�͓����ɂ���
                    AllyIconImages[i].sprite = null;
                    AllyIconTexts[i].text = ally.name;
                    AllyIconImages[i].color = Color.clear;
                }

                // --- 2. �T�C�Y�ƈʒu�̒��� ---
                if (i == 1) // �^��
                {
                    rect.sizeDelta = centerSize;
                    rect.anchoredPosition = Vector2.zero;
                }
                else if (i == 0) // ��
                {
                    rect.sizeDelta = defaultSize;
                    rect.anchoredPosition = new Vector2(-iconSpacing, 0);
                }
                else if (i == 2) // �E
                {
                    rect.sizeDelta = defaultSize;
                    rect.anchoredPosition = new Vector2(iconSpacing, 0);
                }
            }
        }
    }
}