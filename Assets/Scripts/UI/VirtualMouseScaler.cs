using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
#endif
public class VirtualMouseScaler : InputProcessor<Vector2>
{
    public float scale = 1;

    private const string ProcessorName = nameof(VirtualMouseScaler);

#if UNITY_EDITOR
    static VirtualMouseScaler() => Initialize();
#endif

    // Processorの登録処理
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        // 重複登録すると、Input ActionのProcessor一覧に正しく表示されない事があるため、
        // 重複チェックを行う
        if (InputSystem.TryGetProcessor(ProcessorName) == null)
            InputSystem.RegisterProcessor<VirtualMouseScaler>(ProcessorName);
    }

    // 独自のProcessorの処理定義
    public override Vector2 Process(Vector2 value, InputControl control)
    {
        // VirtualMouseから始まるデバイス名ののみ、座標系問題が発生するためProcessorを適用する
        if (control.device.name.StartsWith("VirtualMouse"))
            value *= scale;

        return value;
    }
}