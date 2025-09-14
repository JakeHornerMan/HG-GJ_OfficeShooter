using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SoundConfig
{
    [SerializeField] public AudioClip clip;
    [SerializeField][Range(0f, 1f)] public float volume = 1f;
    [SerializeField] public float topRange = 1f;
    [SerializeField] public float bottomRange = 1f;
}

public class PlayerSounds : MonoBehaviour
{
    [Header("References")]
    public AudioSource audioSrc;
    // [SerializeField] private AudioSource engineSound, skidSound; 
    [SerializeField]
    public SoundConfig
        healthPickUpSound,
        shieldPickUpSound,
        shieldHitSound,
        gunShotSound,
        reloadSound,
        reloadFinsishedSound,
        infoNoise
    ;
    [SerializeField] public SoundConfig[] hurtConfigs;

    [SerializeField] public SoundConfig[] dodgeConfigs;

    // [Header("Settings")]
    // [SerializeField]
    // [Range(0, 1)] private float minPitch = 1f;
    // [SerializeField]
    // [Range(1, 5)] private float maxPitch = 5f;

    void Awake()
    {

    }

    //Single sound plays
    public void PlayHealthPickUpSound() => PlaySoundOnce(healthPickUpSound);
    public void PlayShieldHitSound() => PlaySoundOnce(shieldHitSound);
    public void PlayGunShotSound() => PlaySoundOnce(gunShotSound);
    public void PlayShieldPickUpSound() => PlaySoundOnce(shieldPickUpSound);
    public void PlayReloadSound() => PlaySoundOnce(reloadSound);
    public void PlayReloadFinishedSound() => PlaySoundOnce(reloadFinsishedSound);
    public void PlayInformPlayer() => PlaySoundOnce(infoNoise);

    //Random sound plays
    public void PlayHurtSound() => RandomPlaySound(hurtConfigs);
    public void PlayDodgeSound() => RandomPlaySound(dodgeConfigs);

    private void PlaySoundOnce(SoundConfig soundConfig)
    {
        if (soundConfig == null) return;
        audioSrc.pitch = Random.Range(soundConfig.bottomRange, soundConfig.topRange);
        audioSrc.PlayOneShot(soundConfig.clip, soundConfig.volume / 10);
    }
    
    private void PlaySoundOnceRandomPitch(SoundConfig soundConfig)
    {
        if (soundConfig == null) return;
        audioSrc.pitch = Random.Range(soundConfig.bottomRange, soundConfig.topRange);
        audioSrc.PlayOneShot(soundConfig.clip, soundConfig.volume / 10);
    }

    public void RandomPlaySound(params SoundConfig[] soundConfigs)
    {
        // // if (!Settings.sfxAllowed) return;
        // int willPlaySound = Random.Range(0, 1);
        // if (willPlaySound == 0)
        // {
        if (soundConfigs.Length > 0)
        {
            int randomIndex = Random.Range(0, soundConfigs.Length);
            PlaySoundOnce(soundConfigs[randomIndex]);
        }
        // }
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
