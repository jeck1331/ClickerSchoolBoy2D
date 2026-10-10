using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private GameObject musicSliderGM;
    [SerializeField] private GameObject sfxSliderGM;
    
    [SerializeField] private TMP_Text musicValueText;
    [SerializeField] private TMP_Text sfxValueText;
    
    private Slider _musicSlider;
    private Slider _sfxSlider;
    private bool _isSyncing;

    private void OnEnable()
    {
        GameplayPauseService.SetPaused(this, true);
        transform.SetAsLastSibling();
        _isSyncing = true;
        _musicSlider = musicSliderGM != null ? musicSliderGM.GetComponent<Slider>() : null;
        _sfxSlider = sfxSliderGM != null ? sfxSliderGM.GetComponent<Slider>() : null;
        
        SliderInitialize(_musicSlider, musicValueText, SettingsService.MusicVolume);
        SliderInitialize(_sfxSlider, sfxValueText, SettingsService.SfxVolume);
        SubscribeSlider(_musicSlider, nameof(OnMusicChanged), HandleMusicSliderChanged);
        SubscribeSlider(_sfxSlider, nameof(OnSFXChanged), HandleSfxSliderChanged);
        _isSyncing = false;
    }

    private void OnDisable()
    {
        GameplayPauseService.SetPaused(this, false);
        if (_musicSlider != null) _musicSlider.onValueChanged.RemoveListener(HandleMusicSliderChanged);
        if (_sfxSlider != null) _sfxSlider.onValueChanged.RemoveListener(HandleSfxSliderChanged);
        SettingsService.Save();
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused) SettingsService.Save();
    }

    public void OpenSettings()
    {
        gameObject.SetActive(true);
        GameplayPauseService.SetPaused(this, true);
    }
    
    public void BackToMainMenu()
    {
        GameplayPauseService.SetPaused(this, true);
        
        if (SceneManager.GetActiveScene().name != "StartScene")
            SceneManager.LoadScene(0);
        else 
            ExitSettings();
    }

    public void ExitSettings()
    {
        GameplayPauseService.SetPaused(this, false);
        gameObject.SetActive(false);
    }

    private void SliderInitialize(Slider slider, TMP_Text tmpText, float value)
    {
        if (slider != null)
        {
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.wholeNumbers = false;
            slider.SetValueWithoutNotify(value);
        }
        UpdateValueText(tmpText, value);
    }
    
    public void OnMusicChanged()
    {
        if (_isSyncing || _musicSlider == null || !isActiveAndEnabled) return;
        SettingsService.MusicVolume = _musicSlider.value;
        UpdateValueText(musicValueText, SettingsService.MusicVolume);
        SettingsService.Save();
    }
    
    public void OnSFXChanged()
    {
        if (_isSyncing || _sfxSlider == null || !isActiveAndEnabled) return;
        SettingsService.SfxVolume = _sfxSlider.value;
        UpdateValueText(sfxValueText, SettingsService.SfxVolume);
        SettingsService.Save();
    }

    private void HandleMusicSliderChanged(float value) => OnMusicChanged();
    private void HandleSfxSliderChanged(float value) => OnSFXChanged();
    
    private static void UpdateValueText(TMP_Text tmpText, float value)
    {
        if (tmpText != null) tmpText.text = $"{Mathf.RoundToInt(value * 100f)}%";
    }

    private void SubscribeSlider(Slider slider, string methodName, UnityAction<float> listener)
    {
        if (slider == null) return;
        // Keep existing Inspector bindings without also registering the same callback in code.
        for (var i = 0; i < slider.onValueChanged.GetPersistentEventCount(); i++)
            if (slider.onValueChanged.GetPersistentTarget(i) == this &&
                slider.onValueChanged.GetPersistentMethodName(i) == methodName &&
                slider.onValueChanged.GetPersistentListenerState(i) != UnityEventCallState.Off)
                return;

        slider.onValueChanged.AddListener(listener);
    }
}
