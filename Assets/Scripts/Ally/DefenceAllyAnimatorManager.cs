using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DefAllyAnimater : MonoBehaviour
{
    Animator animator;
    // 判定はCollider同士の衝突で行う例（Trigger を使うなら OnTrigger... に変える）
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        animator.SetBool("DefenceAllyAtack", true);
        Debug.Log("攻撃に移行");
    }
    void OnTriggerStay(Collider other)
    {
        animator.SetBool("DefenceAllyAtack", true);
        Debug.Log("攻撃継続");
    }
    void OnTriggerExit(Collider other)
    {
        animator.SetBool("DefenceAllyClip", false); 
        Debug.Log("通常に移行");
    }
    

    // Trigger を使う場合は以下を使う
    /*
    void OnTriggerEnter(Collider other) { animator.SetBool("DefenceAllyAtack", true); }
    void OnTriggerStay(Collider other)  { animator.SetBool("DefenceAllyAtack", true); }
    void OnTriggerExit(Collider other)  { animator.SetBool("DefenceAllyClip", false); }
    */
}