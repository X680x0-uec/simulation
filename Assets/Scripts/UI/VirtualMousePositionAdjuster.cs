using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

public class VirtualMousePositionAdjuster : MonoBehaviour
{
    [SerializeField] private VirtualMouseInput _virtualMouseInput;
    [SerializeField] private RectTransform cursorTransform;
    [SerializeField] private InputSystemUIInputModule _inputSystemUIInputModule;
    [SerializeField] private Canvas _canvas;

    private Mouse virtualMouse;
    private float _lastScaleFactor = 1;
    
    public bool isActive = false;

    // 現在のCanvasスケール
    private float CurrentScale =>
        _virtualMouseInput.cursorMode == VirtualMouseInput.CursorMode.HardwareCursorIfAvailable
            ? 1
            : _canvas.scaleFactor;

    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == "TitleScene")
        {
            isActive = true;
        }
        else
        {
            isActive = false;
        }
    }

    // Canvasのスケールを監視して、VirtualMouseの座標を補正する
    private void Update()
    {
        // Canvasのスケール取得
        var scale = CurrentScale;

        if (virtualMouse == null)
        {
            virtualMouse = (Mouse)InputSystem.GetDevice("VirtualMouse");
        }
        else
        {
            if (!isActive)
            {
                Vector2 offScreenPos = new Vector2(-9999, -9999);
                InputState.Change(virtualMouse.position, offScreenPos);
                if (cursorTransform != null)
                    cursorTransform.anchoredPosition = offScreenPos;
                return;
            }
            else
            {
                // 画面外に行かないように補正
                Vector2 virtualMousePosition = virtualMouse.position.ReadValue();
                virtualMousePosition.x = Mathf.Clamp(virtualMousePosition.x, 0, _canvas.pixelRect.width);
                virtualMousePosition.y = Mathf.Clamp(virtualMousePosition.y, 0, _canvas.pixelRect.height);
                InputState.Change(virtualMouse.position, virtualMousePosition);
            }
        }

        // スケールが変化した時のみ、以降の処理を実行
        if (Math.Abs(scale - _lastScaleFactor) == 0) return;

        // VirtualMouseInputのカーソルのスケールを変更するProcessorを適用
        _inputSystemUIInputModule.point.action.ApplyBindingOverride(new InputBinding
        {
            overrideProcessors = $"VirtualMouseScaler(scale={scale})"
        });

        _lastScaleFactor = scale;
    }
}