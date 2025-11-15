using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;

    [Header("音量設定")]
    [SerializeField] private TextMeshProUGUI volumeText;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioSource audioSource;
    [Header("サウンド")]
    [SerializeField] private AudioClip settingsOpenClip;


    void Start()
    {
        settingsPanel.SetActive(false);

        float savedVolume = PlayerPrefs.GetFloat("Volume", 1.0f); // 0.0 - 1.0
        int savedPercent = Mathf.RoundToInt(savedVolume * 100f);
        volumeText.text = savedPercent + "%";
        // We expect the slider to be configured 0 - 100 in the inspector
        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 100f;
        volumeSlider.value = savedPercent;
        volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);

        // Ensure a VolumeController exists and apply the saved master volume
        if (VolumeController.Instance == null)
        {
            var go = new GameObject("_VolumeController");
            go.AddComponent<VolumeController>();
            DontDestroyOnLoad(go);
        }
        VolumeController.Instance.SetMasterVolume(savedVolume);

        if (audioSource != null)
        {
            // keep preview/assigned AudioSource in sync
            audioSource.volume = savedVolume;
        }
    }

    public void OnVolumeSliderChanged(float value)
    {
        int percent = Mathf.RoundToInt(value);
        volumeText.text = percent + "%";
        float master = Mathf.Clamp01(percent / 100f);
        PlayerPrefs.SetFloat("Volume", master);

        // Apply to master controller which preserves per-source relative volumes
        if (VolumeController.Instance == null)
        {
            var go = new GameObject("_VolumeController");
            go.AddComponent<VolumeController>();
            DontDestroyOnLoad(go);
        }
        VolumeController.Instance.SetMasterVolume(master);

        if (audioSource != null)
        {
            // keep preview/assigned AudioSource in sync
            audioSource.volume = master;
        }
    }

    public void SetPanelActive()
    {
        settingsPanel.SetActive(true);
        // Play assigned sound when opening settings
        if (settingsOpenClip != null)
        {
            if (audioSource != null)
            {
                audioSource.PlayOneShot(settingsOpenClip);
            }
            else
            {
                // fallback: play at camera position so it's heard in 2D projects as well
                var camPos = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
                AudioSource.PlayClipAtPoint(settingsOpenClip, camPos);
            }
        }
    }

    public void SetPanelInactive()
    {
        settingsPanel.SetActive(false);
    }
}