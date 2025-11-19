using UnityEngine;
using UnityEngine.EventSystems;

[DefaultExecutionOrder(-1000)] // できるだけ早く実行して EventSystem.OnEnable より前に処理する
public class EventSystemGuard : MonoBehaviour
{
    void Awake()
    {
        // 既に有効な EventSystem が存在するなら自分を破棄する
        if (EventSystem.current != null && EventSystem.current.gameObject != gameObject)
        {
            Debug.Log($"[EventSystemGuard] Another EventSystem exists ({EventSystem.current.gameObject.name}). Destroying this one: {gameObject.name}");
            Destroy(gameObject);
            return;
        }

        // 自分が残る場合は current に設定されるようにする（念のため）
        var es = GetComponent<EventSystem>();
        if (es != null && EventSystem.current == null)
        {
            // EventSystem.current は内部で設定されるが、念のためログを出す
            Debug.Log($"[EventSystemGuard] Keeping EventSystem on {gameObject.name}");
        }
    }
}