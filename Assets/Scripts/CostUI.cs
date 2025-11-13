using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CostUI : MonoBehaviour
{
    private TextMeshProUGUI txt;
    private RectTransform rect;

    [Header("配置設定")]
    public Vector2 padding = new Vector2(10f, 10f); // 右上からの余白（px）

    void Awake()
    {
        txt = GetComponent<TextMeshProUGUI>();
        rect = GetComponent<RectTransform>();
        // 親がCanvasの下にある前提で右上にアンカー設定
        if (rect != null && rect.parent != null)
        {
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = new Vector2(-padding.x, -padding.y);
        }
    }

    void Update()
    {
        if (CostManager.Instance != null)
        {
            txt.text = $"Points: {CostManager.Instance.GetCurrentPoints()}";
        }
        else
        {
            txt.text = "Points: (no CostManager)";
        }
    }
}
