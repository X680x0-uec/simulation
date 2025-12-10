using UnityEngine;

/// <summary>
/// 実行時にアスペクト比を固定するコンポーネント
/// カメラのViewportを調整して黒帯を追加
/// </summary>
[RequireComponent(typeof(Camera))]
public class FixedAspectRatio : MonoBehaviour
{
    [SerializeField]
	Camera targetCamera;
	[SerializeField]
	Vector2 targetSize = new(16f, 9f);
	[SerializeField]
	bool autoSetOnStart = true;

	public Camera TargetCamera { get { return targetCamera; } set { targetCamera = value; } }
	public Vector2 TargetSize { get {  return targetSize; } set { targetSize = value; } }

	void Awake()
	{
		if (autoSetOnStart)
		{
			if (targetCamera == null)
			{
				targetCamera = GetComponent<Camera>();
			}
			SetAspectRatio();
		}
	}

	public void SetAspectRatio()
	{
		if(targetCamera == null)
		{
			return;
		}

		float currentRatio = (float)Screen.width / Screen.height;
		float targetRatio = targetSize.x / targetSize.y;
		float scaleHeight = currentRatio / targetRatio;
		Rect rect = targetCamera.rect;

		if (scaleHeight < 1f)
		{
			rect.width = 1f;
			rect.height = scaleHeight;
			rect.x = 0;
			rect.y = (1f - scaleHeight) / 2f;
		}
		else
		{
			float scaleWidth = 1f / scaleHeight;
			rect.width = scaleWidth;
			rect.height = 1f;
			rect.x = (1f - scaleWidth) / 2f;
			rect.y = 0;
		}

	targetCamera.rect = rect;
	}

	private float lastAspect; // 前回のアスペクト比
    private int lastWidth;
    private int lastHeight;

    void Start()
    {
        lastWidth = Screen.width;
        lastHeight = Screen.height;
        lastAspect = GetAspectRatio();
    }

    void Update()
    {
        // 解像度が変わったかチェック
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            float currentAspect = GetAspectRatio();

            if (!Mathf.Approximately(currentAspect, lastAspect))
            {
                Debug.Log($"アスペクト比が変化しました: {lastAspect:F3} → {currentAspect:F3}");
                OnAspectRatioChanged(lastAspect, currentAspect);
                lastAspect = currentAspect;
            }

            lastWidth = Screen.width;
            lastHeight = Screen.height;
        }
    }

    /// <summary>
    /// アスペクト比を計算
    /// </summary>
    private float GetAspectRatio()
    {
        return (float)Screen.width / Screen.height;
    }

    /// <summary>
    /// アスペクト比が変わったときの処理
    /// </summary>
    private void OnAspectRatioChanged(float oldAspect, float newAspect)
    {
        // ここにレイアウト調整やカメラ設定変更などの処理を書く
		SetAspectRatio();
    }
}
