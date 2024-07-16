using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [field: SerializeField] public AudioClip[] BackgroundMusic
    { get; private set; }
    public AudioSource MusicPlayer
    { get; private set; }
    public float DefaultVolume
    { get; private set; } = 0.05f;
    public float MaximumVolumeMultiplier
    { get; private set; } = 3.0f;
    public int CurrentClip
    { get; private set; } = 0;

    public VolumeControlObjects[] ExternalAudioSources
    { get; private set; }


    [field: SerializeField] public AudioClip ObjectGained
    { get; private set; }
    [field: SerializeField] public AudioClip ObjectLost
    { get; private set; }
    [field: SerializeField] public AudioClip GameSuccess
    { get; private set; }

    private void Awake()
    {
        MusicPlayer = GetComponent<AudioSource>();
        ExternalAudioSources = FindObjectsOfType<VolumeControlObjects>();
    }

    // Start is called before the first frame update
    void Start()
    {
        MusicPlayer.volume = DefaultVolume;
        MusicPlayer.clip = BackgroundMusic[CurrentClip];
        MusicPlayer.Play();
    }

    // Update is called once per frame
    void Update()
    {
        ChangeClip();
    }

    void ChangeClip()
    {
        if (MusicPlayer.isPlaying == false)
        {
            CurrentClip++;
            if (CurrentClip == BackgroundMusic.Length)
            {
                CurrentClip = 0;
            }

            MusicPlayer.clip = BackgroundMusic[CurrentClip];
            MusicPlayer.Play();
        }
    }

    public void ChangeVolume(float multiplier)
    {
        MusicPlayer.volume = DefaultVolume * multiplier;

        foreach (VolumeControlObjects element in ExternalAudioSources)
        {
            float currentDefault = element.DefaultVolume;
            AudioSource currentAudioSource = element.GetComponent<AudioSource>();
            currentAudioSource.volume = currentDefault * multiplier;
        }
        
    }

    public void PlayOneShotSound(AudioClip clip)
    {
        MusicPlayer.PlayOneShot(clip, 2f);
    }
}
