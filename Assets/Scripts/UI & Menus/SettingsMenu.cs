using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SettingsMenu : MonoBehaviour {

    [Header("Audio")]

    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private SliderData _masterVol;
    [SerializeField] private SliderData _musicVol;
    [SerializeField] private SliderData _sfxVol;
    private float _currentSliderValue;

    private void Start() {
        // Sets GUI sliders to what is written in the GM
        _masterVol.slider.value = GameManager.MasterVol;
        _musicVol.slider.value = GameManager.MusicVol;
        _sfxVol.slider.value = GameManager.SfxVol;
        _masterVol.valueText.text = (GameManager.MasterVol * 100).ToString("F0");
        _musicVol.valueText.text = (GameManager.MusicVol * 100).ToString("F0");
        _sfxVol.valueText.text = (GameManager.SfxVol * 100).ToString("F0");
    }

    // Adjusts the volume 'volToChange' to it's slider's value, and it's corresponding text
    public void SetVolume(int volToChange) {
        SliderData slider = GetSliderVol(volToChange);
        _mixer.SetFloat(slider.name, Mathf.Log10(_currentSliderValue) * 20); // Slider lowest must be 0.001
        GetManagerVol(volToChange) = Mathf.Round(slider.slider.value * 1000f) / 1000f;
        slider.valueText.text = (slider.slider.value * 100f).ToString("F0");
    }

    // Correlates an Int to a GM variable
    private ref float GetManagerVol(int volVarID) {
        switch (volVarID) {
            case 0: return ref GameManager.MasterVol;
            case 1: return ref GameManager.MusicVol;
            case 2: return ref GameManager.SfxVol;
            default:
                Debug.Log("Error Fetching Volume Var From Manager, used MasterVol Instead");
                return ref GameManager.MasterVol;
        }
    }

    // Correlates an Int to a slider in the GUI
    private SliderData GetSliderVol(int volVarID) {
        switch (volVarID) {
            case 0: return _masterVol;
            case 1: return _musicVol;
            case 2: return _sfxVol;
            default:
                Debug.Log("Error Fetching Volume Slider Value, used MasterVolSlider Instead");
                return _masterVol;
        }
    }
}

// Stores relevant data of a slider
[System.Serializable]
public class SliderData {
    public string name;
    public Slider slider;
    public TextMeshProUGUI valueText;
}