using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SoundConfig
{
    [SerializeField] public AudioClip clip;
    [SerializeField][Range(0f, 1f)] public float volume = 1f;
}

public class PlayerSounds : MonoBehaviour
{
    [Header("References")]
    public AudioSource audioSrc;
    // [SerializeField] private AudioSource engineSound, skidSound; 
    [SerializeField] private SoundConfig hurtSound, healSound, shieldSound;

    [Header("Settings")]
    [SerializeField]
    [Range(0, 1)] private float minPitch = 1f;
    [SerializeField]
    [Range(1, 5)] private float maxPitch = 5f;

    void Awake()
    {

    }

    public void PlayBoostSound() => PlaySoundOnce(boostSound);

    private void PlaySoundOnce(SoundConfig soundConfig)
    {
        if (soundConfig == null) return;
        audioSrc.PlayOneShot(soundConfig.clip, soundConfig.volume / 10);
    }

    // public void EngineSound(float carVelocityRatio)
    // {
    //     engineSound.pitch = Mathf.Lerp(minPitch, maxPitch, Mathf.Abs(carVelocityRatio));
    // }
    // public void ToggleSkidSound(bool toggle)
    // {
    //     skidSound.mute = !toggle;
    // }
}
