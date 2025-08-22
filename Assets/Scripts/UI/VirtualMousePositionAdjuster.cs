using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class SoftwareCursorPositionAdjuster : MonoBehaviour
{
    [SerializeField] private VirtualMouseInput _virtualMouse;
    [SerializeField] private InputSystemUIInputModule _inputSystemUIInputModule;
    [SerializeField] private Canvas _canvas;

    private float _lastScaleFactor = 1;

    // 現在のCanvasスケール
    private float CurrentScale =>
        _virtualMouse.cursorMode == VirtualMouseInput.CursorMode.HardwareCursorIfAvailable
            ? 1
            : _canvas.scaleFactor;

    // Canvasのスケールを監視して、VirtualMouseの座標を補正する
    private void Update()
    {
        // Canvasのスケール取得
        var scale = CurrentScale;

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