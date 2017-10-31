using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager {
    AudioSource source;
    public void Initialize()
    {
        source = UberManager.Instance.gameObject.AddComponent<AudioSource>();

        SetBackGroundMusic("Sound/Background");
    }

    public void SetBackGroundMusic(string file)
    {
        AudioClip backgroundMusic = Resources.Load(file) as AudioClip;
        source.Stop();
        source.clip = backgroundMusic;
        source.loop = true;
        source.Play();
    }
}
