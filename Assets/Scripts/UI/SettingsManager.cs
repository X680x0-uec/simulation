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


    void Start()
    {
        settingsPanel.SetActive(false);

        float savedVolume = PlayerPrefs.GetFloat("Volume", 1.0f);
        volumeText.text = savedVolume * 100 + "%";
        volumeSlider.value = savedVolume;
        volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);
        if (audioSource != null)
        {
            audioSource.volume = savedVolume;
        }
    }

    public void OnVolumeSliderChanged(float value)
    {
        value = Mathf.RoundToInt(value);
        volumeText.text = value + "%";
        PlayerPrefs.SetFloat("Volume", value / 100f);
        if (audioSource != null)
        {
            audioSource.volume = value / 100f;
        }
    }

    public void SetPanelActive()
    {
        settingsPanel.SetActive(true);
    }

    public void SetPanelInactive()
    {
        settingsPanel.SetActive(false);
    }
}