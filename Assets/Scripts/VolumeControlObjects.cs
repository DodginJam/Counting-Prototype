using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeControlObjects : MonoBehaviour
{
    [field: SerializeField] public float DefaultVolume
    { get; private set; }
    public AudioSource AudioOutput
    { get; private set; }
    [field: SerializeField] public AudioClip AudioClipToPlay
    { get; private set; }

    private void Awake()
    {
        AudioOutput = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // All audio for trucks and generator (AudioSources seperate from non-spatial) have been disabled, i.e. no AudioClip assigned, due to issue with the Audio for these not abiding by the Volume Control system set up.
        // Issue is that this only occurs in the WEBGL build running on the site - the Volume Control for everything, trucks and generator included, worked fine in the Unity editor.
        /*
        AudioOutput.volume = DefaultVolume;
        AudioOutput.clip = AudioClipToPlay;
        AudioOutput.Play();
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
