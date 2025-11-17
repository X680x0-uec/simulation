using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Singleton（シングルトン）設定
    public static SoundManager instance = null;

    // インスペクタで設定するSEファイル
    [Header("攻撃された時の音")]
    [SerializeField] private AudioClip mikoshiAttackedSound;
    [SerializeField] private AudioClip allyAttackedSound;
    [SerializeField] private AudioClip enemyAttackedSound;

    [Header("撃破された時の音")]
    [SerializeField] private AudioClip allyDefeatedSound;
    [SerializeField] private AudioClip enemyDefeatedSound;

    [Header("音量設定")]
    [SerializeField, Range(0, 1f)] // 0から1の範囲で指定
    private float seVolume = 0.8f; // デフォルトの音量を0.8に設定

    // 自分のAudioSourceコンポーネント
    private AudioSource audioSource;

    void Awake()
    {
        // --- Singletonパターンの実装 ---
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        // ----------------------------------------------------

        // 自分のAudioSourceを取得
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("SoundManagerにAudioSourceがありません！");
        }
    }

    // 呼び出し用のメソッド (2D再生なので位置情報は不要)

    // ミコシが攻撃された時に呼ぶ
    public void PlayMikoshiAttacked()
    {
        if (mikoshiAttackedSound != null)
            audioSource.PlayOneShot(mikoshiAttackedSound, seVolume);
    }

    // 味方(Ally)が攻撃された時に呼ぶ
    public void PlayAllyAttacked()
    {
        if (allyAttackedSound != null)
            audioSource.PlayOneShot(allyAttackedSound, seVolume);
    }

    // 敵(Enemy)が攻撃された時に呼ぶ
    public void PlayEnemyAttacked()
    {
        if (enemyAttackedSound != null)
            audioSource.PlayOneShot(enemyAttackedSound, seVolume);
    }

    // 味方(Ally)が撃破された時に呼ぶ
    public void PlayAllyDefeated()
    {
        if (allyDefeatedSound != null)
            audioSource.PlayOneShot(allyDefeatedSound, seVolume);
    }

    // 敵(Enemy)が撃破された時に呼ぶ
    public void PlayEnemyDefeated()
    {
        if (enemyDefeatedSound != null)
            audioSource.PlayOneShot(enemyDefeatedSound, seVolume);
    }
}