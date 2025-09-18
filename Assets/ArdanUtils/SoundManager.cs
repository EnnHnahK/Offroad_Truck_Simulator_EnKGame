using System.Collections.Generic;
using Lofelt.NiceVibrations;
using UnityEngine;
//using Lofelt.NiceVibrations;

public enum CurrentTime
{
    Day,
    Afternoon,
    Night
}

public enum MusicType
{
    Gameplay,
    Menu
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    
    [Header("Music")]
    //public AudioSource musicSource;
    //public List<AudioClip> clipBgs;
    [Space]
    [Header("Sound")]
    public AudioSource[] soundSources;
    public AudioSource[] loopSources;
    
    [Space]
    [Header("Clip weather")]
    public AudioClip rain;
    [Space]
    [Header("Clip car")]
    //public AudioClip engine;
    public AudioClip[] crash;
    public AudioClip gearShift;//, bakingUp;
    public AudioClip carPuddle, fallLake;
    public AudioClip coin;
    public AudioClip truckFire, water;
    
    [Space]
    [Header("Clip UI")]
    public AudioClip click;
    public AudioClip panelShow;
    public AudioClip[] upgradeSound;
    
    private Queue<AudioSource> _queueSources;
    private Queue<AudioSource> _queueLoops;
    private Dictionary<AudioClip, AudioSource> _dicLoop = new();
    
    
    protected virtual void Awake()
    {
        Instance = this;
        _queueSources = new Queue<AudioSource>(soundSources);
        _queueLoops = new Queue<AudioSource>(loopSources);

        AudioListener.volume = 0.5f;
        
        UpdateMute();
    }
    

    // public void PlayBg(MusicType musicType)
    // {
    //     switch (musicType)
    //     {
    //         case MusicType.Menu when musicSource.clip == clipBgs[0]:
    //             return;
    //         case MusicType.Menu:
    //             musicSource.clip = clipBgs[0];
    //             musicSource.Play();
    //             break;
    //         case MusicType.Gameplay when musicSource.clip == clipBgs[1]:
    //             return;
    //         case MusicType.Gameplay:
    //             musicSource.clip = clipBgs[1];
    //             musicSource.Play();
    //             break;
    //     }
    // }

    public void RandomCrash()
    {
        PlayEmphasis(.2f, .2f);
        PlayShot(crash[Random.Range(0, crash.Length)], .7f);
    }

    public void UpgradeSound(int index)
    {
        PlayShot(upgradeSound[index]);
    }
    
    public void UpdateMute()
    {
        ChangeSound();
        ChangeMusic();
    }

    void ChangeSound()
    {
        var mute = !DataController.Sound;

        foreach (var sound in soundSources)
        {
            sound.mute = mute;
        }
        foreach (var sound in loopSources)
        {
            sound.mute = mute;
        }
        //_TruckManager.Instance?.GetVehicleAudio().UpdateMute(mute);
    }

    private void ChangeMusic()
    {
        var mute = !DataController.Music;
        // if (musicSource != null)
        // {
        //     musicSource.mute = mute;
        // }
    }
    
    
    public virtual void PlayShot(AudioClip clip, float volume = 1f)
    {
        // if (!UIController.Instance.uiGamePlay.showing)
        // {
        //     return;
        // }
        if (clip == null)
        {
            return;
        }
        var source = _queueSources.Dequeue();
        source.volume = volume;
        source.PlayOneShot(clip);
        _queueSources.Enqueue(source);
    }
    
    // public void PlayVibrate()
    // {
    //     if (!PrefData.Vibrate)
    //     {
    //         return;
    //     }
    //     Handheld.Vibrate();
    // }
    //
    // public void PlayVibrate(long milliseconds = 50)
    // {
    //     if (!PrefData.Vibrate)
    //     {
    //         return;
    //     }
    //     Vibration.Vibrate(milliseconds);
    // }
    
    public void PlayEmphasis(float amplitude, float frequency)
    {
        if (!PrefData.Vibrate)
        {
            return;
        }
        HapticPatterns.PlayEmphasis(amplitude, frequency);
    }

    public void PlayConstant(float amplitude, float frequency, float duration)
    {
        
        if (!PrefData.Vibrate)
        {
            return;
        }
        HapticPatterns.PlayConstant(amplitude, frequency, duration);
    }
    
    public void PlayLoop(AudioClip clip, float volume = 1f)
    {
        if (clip == null || _dicLoop.ContainsKey(clip))
        {
            return;
        }
        var loopSource = _queueLoops.Dequeue();
        loopSource.volume = volume;
        loopSource.clip = clip;
        loopSource.Play();
        _dicLoop.Add(clip, loopSource);
    }

    public void StopLoop(AudioClip clip)
    {
        if (!clip || !_dicLoop.ContainsKey(clip))
        {
            return;
        }
        var loopSource = _dicLoop[clip];
        if (loopSource)
        {
            loopSource.Stop();
            _queueLoops.Enqueue(loopSource);
            _dicLoop.Remove(clip);
        }
    }
}
