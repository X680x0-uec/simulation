using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class MouseSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject cursorObject;
    [SerializeField] private VirtualMouseInput virtualMouseInput;

    private bool usingPhysicalMouse;

    void Update()
    {
        // 物理マウスの入力チェック
        if (Mouse.current != null)
        {
            if (Mouse.current.delta.ReadValue() != Vector2.zero || Mouse.current.leftButton.isPressed)
            {
                if (!usingPhysicalMouse)
                {
                    usingPhysicalMouse = true;
                    SetVirtualMouseVisible(false);
                }
            }
        }

        // ゲームパッド入力チェック（右スティックなどで動かす場合）
        if (Gamepad.current != null)
        {
            if
            (
                Gamepad.current.leftStick.ReadValue().sqrMagnitude > 0.01f ||
                Gamepad.current.rightStick.ReadValue().sqrMagnitude > 0.01f ||
                Gamepad.current.buttonSouth.isPressed ||
                Gamepad.current.buttonEast.isPressed ||
                Gamepad.current.buttonNorth.isPressed ||
                Gamepad.current.buttonWest.isPressed
            )
            {
                if (usingPhysicalMouse)
                {
                    usingPhysicalMouse = false;
                    SetVirtualMouseVisible(true);
                }
            }
        }
    }

    private void SetVirtualMouseVisible(bool visible)
    {
        if (cursorObject != null)
            cursorObject.SetActive(visible);
    }
}