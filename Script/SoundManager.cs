using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private float volume = 1f;
    private const string PLAYER_PREF_SOUND_EFFECT = "player_pref_sound_effect";
    private const string PLAYER_PREF_EFFECT_VOLUME = "effect_volume";
    [SerializeField] private CarController carController;
    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 2.0f;
    [SerializeField] private float offMinPitch = 0.2f; 
    [SerializeField] private float pitchChangeSpeed = 2.5f;
    [SerializeField] private float effectVolume = 1f;
    [SerializeField] private AudioSource engineAudioSource;
    [SerializeField] private AudioSource effectAudioSource;
    private float maxEffectVolume = 10f;
    private float targetEnginePitch;

    public void Awake()
    {
        Instance = this;

    }

    public void Start()
    {
        targetEnginePitch = minPitch - offMinPitch;
        effectVolume = PlayerPrefs.GetFloat(PLAYER_PREF_EFFECT_VOLUME, 1f);
        ApplyVolume();
    }

    public void Update()
    {
        if (carController == null) return;
        UpdateEngineSoundSmoothly();
    }

    private void UpdateEngineSoundSmoothly()
    {
        float currentSpeed = Mathf.Abs(carController.GetVelocity());
        float maxSpeed = carController.GetMaxSpeed();
        float speedNormalized = Mathf.Clamp01(currentSpeed / maxSpeed);

        switch (carController.GetState())
        {
            case CarController.State.Idle:
                targetEnginePitch = minPitch - offMinPitch;
                break;

            case CarController.State.ForwardLowSpeed:
            case CarController.State.ForwardHighSpeed:
            case CarController.State.ForwardMaxSpeed:
            case CarController.State.Reverse:
                targetEnginePitch = Mathf.Lerp(minPitch, maxPitch, speedNormalized);
                break;
        }

        if (engineAudioSource != null && engineAudioSource.isPlaying)
        {
            engineAudioSource.pitch = Mathf.MoveTowards(
                engineAudioSource.pitch,
                targetEnginePitch,
                pitchChangeSpeed * Time.deltaTime
            );
        }
    }
    public void SetEffectVolume(float value)
    {
        effectVolume = Mathf.Clamp01(value);

        ApplyVolume();

        PlayerPrefs.SetFloat(PLAYER_PREF_EFFECT_VOLUME, effectVolume);
        PlayerPrefs.Save();
    }
    public float GetEffectVolume()
    {
        return (float)PlayerPrefs.GetFloat(PLAYER_PREF_EFFECT_VOLUME) / maxEffectVolume;
    }


    private void ApplyVolume()
    {
        if (engineAudioSource != null)
            engineAudioSource.volume = effectVolume;

        if (effectAudioSource != null)
            effectAudioSource.volume = effectVolume;
    }
    private void CarController_OnStateChange(object sender, EventArgs e)
    {
        ChangeState();
    }

    public void ChangeState()
    {
        if (carController == null || carController.carSO == null) return;

        var soundSO = carController.carSO.soundEffectCarSO;

        switch (carController.GetState())
        {
            case CarController.State.Idle:
                SetEngineClip(soundSO.engineSound);
                break;

            case CarController.State.ForwardLowSpeed:
            case CarController.State.ForwardHighSpeed:
            case CarController.State.ForwardMaxSpeed:
            case CarController.State.Reverse:
                SetEngineClip(soundSO.drive != null ? soundSO.drive : soundSO.engineSound);
                break;
        }
    }

    private void SetEngineClip(AudioClip clip)
    {
        if (clip == null || engineAudioSource == null) return;

        if (engineAudioSource.clip != clip)
        {
            engineAudioSource.clip = clip;
            engineAudioSource.Play();
        }
        else if (!engineAudioSource.isPlaying)
        {
            engineAudioSource.Play();
        }
    }

    private void Instance_OnHornActive(object sender, EventArgs e)
    {
        if (carController != null && carController.carSO != null && carController.carSO.soundEffectCarSO.horn != null)
        {
            PlaySound(carController.carSO.soundEffectCarSO.horn, carController.transform.position);
        }
    }

    public void PlaySound(AudioClip clip, Vector3 position, float volumeMultiply = 1f)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, position, volume * volumeMultiply);
    }

    public float GetVolume()
    {
        return PlayerPrefs.GetFloat(PLAYER_PREF_EFFECT_VOLUME, 1f);
    }

    public void SetCar(CarController car)
    {
        if (carController != null)
        {
            carController.OnStateChange -= CarController_OnStateChange;
        }

        carController = car;
        carController.OnStateChange += CarController_OnStateChange;

        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnHornActive -= Instance_OnHornActive;
            GameInput.Instance.OnHornActive += Instance_OnHornActive;
        }

        ChangeState();
    }

    private void OnDestroy()
    {
        if (carController != null)
        {
            carController.OnStateChange -= CarController_OnStateChange;
        }
        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnHornActive -= Instance_OnHornActive;
        }
    }
}