using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour {

    [SerializeField] AudioType audioType = AudioType.MusicMixer;

    [SerializeField] private AudioMixer audioMasterMixer;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Toggle soundToggle;

    [Header("Current State")]
    [SerializeField] AudioState audioState = AudioState.InitialState;

    private VolumeSavedSettings volumeSettings = VolumeSavedSettings.Get();

    [Range(0.1f, 3f)] [SerializeField] float SoundAmplifier = 1f;
    private const float MinSliderThreshold = 0.00000000000001f;
    private const float MinBaseValue = -80f;

    float storedVolume;

    public enum AudioType {
        MusicMixer,
        SfxMixer,
    }

    public enum AudioState {
        InitialState,
        EnableState,
        DisableState,
        SwipeState,
        IdleState,
    }

    private void Start() {
        InitiateState();
    }

    private void OnEnable() {
        soundToggle.onValueChanged.AddListener(OnToggleChanged);
        soundSlider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void OnDisable() {
        soundToggle.onValueChanged.RemoveListener(OnToggleChanged);
        soundSlider.onValueChanged.RemoveListener(OnSliderChanged);
    }

    public string GetAudioTypeName() {
        return audioType == AudioType.MusicMixer ? "MusicMixer" : "SfxMixer";
    }

    public void SaveSettings() {
        UnityJsonManager.SaveToJSON(GetAudioTypeName(), volumeSettings);
    }

    public void LoadSettings() {
        UnityJsonManager.GetFromJSON(GetAudioTypeName(), volumeSettings);
    }

    public void OnEnterState(AudioState state) {
        switch (state) {
            case AudioState.InitialState:
                InitiateState();
                break;
            case AudioState.EnableState:
                EnableState();
                break;
            case AudioState.DisableState:
                DisableState();
                break;
            case AudioState.SwipeState:
                SwipeState();
                break;
            case AudioState.IdleState:
                IdleState();
                break;
        }
    }

    public void ChangeState(AudioState state) {
        audioState = state;
        OnEnterState(audioState);
    }

    public void InitiateState() {
        LoadSettings();
        storedVolume = volumeSettings.Volume;

        Debug.LogWarning($"{GetAudioTypeName()} Volume Settings > {volumeSettings.IsOn} and {volumeSettings.Volume}");

        if (volumeSettings.IsOn == true) { ChangeState(AudioState.EnableState); }
        else { ChangeState(AudioState.DisableState); }
    }

    public void EnableState() {

        if (storedVolume == 0) {
            soundToggle.SetIsOnWithoutNotify(false);
            return;
        }

        soundToggle.SetIsOnWithoutNotify(true);
        soundSlider.SetValueWithoutNotify(storedVolume);
        volumeSettings.IsOn = true;
        volumeSettings.Volume = storedVolume;
        SetVolume(storedVolume);
        SaveSettings();
    }

    public void DisableState() {
        soundToggle.SetIsOnWithoutNotify(false);
        soundSlider.SetValueWithoutNotify(storedVolume);
        volumeSettings.IsOn = false;
        volumeSettings.Volume = storedVolume;
        SetVolume(0);
        SaveSettings();
    }

    public void SwipeState() {

        if(soundSlider.value == 0) {
            storedVolume = volumeSettings.Volume = soundSlider.value;
            volumeSettings.IsOn = false;
            soundToggle.SetIsOnWithoutNotify(false);
            soundSlider.SetValueWithoutNotify(soundSlider.value);
        }
        else {
            volumeSettings.Volume = storedVolume = soundSlider.value;
            volumeSettings.IsOn = true;
            soundToggle.SetIsOnWithoutNotify(true);
        }

        SetVolume(storedVolume);
        SaveSettings();
    }

    protected void IdleState() {
        //Idle Nothing
    }

    private void SetVolume(float volume) {

        if (volume < MinSliderThreshold) {
            volume = MinBaseValue;
        }
        else {
            volume = Mathf.Log(volume, 1.05f) * SoundAmplifier;
        }
        audioMasterMixer.SetFloat(GetAudioTypeName(), volume);
    }

    private void OnToggleChanged(bool isOn) {
        ChangeState(isOn ? AudioState.EnableState : AudioState.DisableState);
    }

    private void OnSliderChanged(float value) {
        ChangeState(AudioState.SwipeState);
    }

    [ContextMenu("Get Volume Save Info")]
    private void GetVolumeSavedFile() {
        LoadSettings();
        Debug.LogAssertion($"Volume Settings Info > {volumeSettings.IsOn} and {volumeSettings.Volume}");
    }
}