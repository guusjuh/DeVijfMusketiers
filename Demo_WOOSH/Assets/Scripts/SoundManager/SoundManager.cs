using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager {
    private static AudioSource soundEffectSource;
    private static AudioSource backgroundSource;

    private static Dictionary<SoundEffect, AudioClip> soundEffects = new Dictionary<SoundEffect, AudioClip>();
    private static Dictionary<Background, AudioClip> backgroundMusic = new Dictionary<Background, AudioClip>();

    private static bool musicOn = true;
    public static bool MusicOn
    {
        get { return musicOn; }
        set
        {
            musicOn = value;
            backgroundSource.mute = !musicOn;
        }
    }

    private static bool fxOn = true;
    public static bool FXOn
    {
        get { return fxOn; }
        set
        {
            fxOn = value;
            soundEffectSource.mute = !fxOn;
        }
    }

    public enum Background
    {
        InGame = 0,
        LevelSelection
    }

    public enum SoundEffect
    {
        AttackPlayer = 0,
        FirePlayer,
        FreezePlayer,
        TeleportPlayer,
        ButtonClick,
        DyingHuman,
        Gap,
        Shrine,
        Burn,
        Victory,
        Defeat
    }
    /*
     * heal
     * block
     * firebal dodin
     */

    public static void Initialize()
    {
        soundEffectSource = UberManager.Instance.gameObject.AddComponent<AudioSource>();
        backgroundSource = UberManager.Instance.gameObject.AddComponent<AudioSource>();

        backgroundMusic.Add(Background.InGame, Resources.Load<AudioClip>("Sound/BackgroundMusic/InGame"));
        backgroundMusic.Add(Background.LevelSelection, Resources.Load<AudioClip>("Sound/BackgroundMusic/LevelSelect"));

        soundEffects.Add(SoundEffect.AttackPlayer, Resources.Load("Sound/SoundEffects/attackPlayer") as AudioClip);
        soundEffects.Add(SoundEffect.FirePlayer, Resources.Load("Sound/SoundEffects/firePlayer") as AudioClip);
        soundEffects.Add(SoundEffect.FreezePlayer, Resources.Load("Sound/SoundEffects/freezePlayer") as AudioClip);
        soundEffects.Add(SoundEffect.TeleportPlayer, Resources.Load("Sound/SoundEffects/teleportPlayer") as AudioClip);
        soundEffects.Add(SoundEffect.ButtonClick, Resources.Load("Sound/SoundEffects/buttonClick") as AudioClip);
        soundEffects.Add(SoundEffect.DyingHuman, Resources.Load("Sound/SoundEffects/dyingHuman") as AudioClip);
        soundEffects.Add(SoundEffect.Gap, Resources.Load("Sound/SoundEffects/gap") as AudioClip);
        soundEffects.Add(SoundEffect.Shrine, Resources.Load("Sound/SoundEffects/shrine") as AudioClip);
        soundEffects.Add(SoundEffect.Burn, Resources.Load("Sound/SoundEffects/burn") as AudioClip);
        soundEffects.Add(SoundEffect.Victory, Resources.Load("Sound/SoundEffects/victory") as AudioClip);
        soundEffects.Add(SoundEffect.Defeat, Resources.Load("Sound/SoundEffects/defeat") as AudioClip);

        SetBackGroundMusic(Background.LevelSelection);
    }

    public static void SetBackGroundMusic(Background music)
    {
        backgroundSource.Stop();
        backgroundSource.clip = backgroundMusic[music];
        backgroundSource.loop = true;
        backgroundSource.Play();
    }

    public static void PlaySoundEffect(GameManager.SpellType effect)
    {
        switch (effect)
        {
            case GameManager.SpellType.Attack:
                PlaySoundEffect(SoundEffect.AttackPlayer);
                break;
            case GameManager.SpellType.Fireball:
                PlaySoundEffect(SoundEffect.FirePlayer);
                break;
            case GameManager.SpellType.FrostBite:
                PlaySoundEffect(SoundEffect.FreezePlayer);
                break;
            case GameManager.SpellType.Teleport:
                PlaySoundEffect(SoundEffect.TeleportPlayer);
                break;
        }
    }

    public static void PlaySoundEffect(SoundEffect effect)
    {
        soundEffectSource.PlayOneShot(soundEffects[effect]);
    }

    public static void PlaySoundEffect()
    {
        PlaySoundEffect(SoundEffect.ButtonClick);
    }
}
