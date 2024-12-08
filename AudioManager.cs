using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance { get; private set; }  

    private const string musicStatus = "MusicStatus";
    private const string sfxStatus = "SFXStatus";
    private const int maxSFXPlayers = 5;

    [SerializeField] Music[] musics;
    [SerializeField] Sound[] sounds;

    AudioSource musicPlayer;
    List<AudioSource> sfxPlayers;

    public bool IsMusicOn
    {
        get => PlayerPrefs.GetInt(musicStatus, 1) == 1;
        set => PlayerPrefs.SetInt(musicStatus, value ? 1 : 0);
    }

    public bool IsSoundOn
    {
        get => PlayerPrefs.GetInt(sfxStatus, 1) == 1;
        set => PlayerPrefs.SetInt(sfxStatus, value ? 1 : 0);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        musicPlayer = gameObject.AddComponent<AudioSource>();
        musicPlayer.loop = true;

        for (int i = 0; i < 3; i++)
            sfxPlayers.Add(gameObject.AddComponent<AudioSource>());

        if (IsMusicOn)
            PlayMusic(MusicType.HomeMusic);
    }

    public void ToggleMusic()
    {
        IsMusicOn = !IsMusicOn;
        musicPlayer.mute = !IsMusicOn;

        if(IsMusicOn && !musicPlayer.isPlaying)
            musicPlayer.Play();
        else if(!IsMusicOn)
            musicPlayer.Pause();
    }

    public void ToggleSFX()
    {
        IsSoundOn = !IsSoundOn;
        sfxPlayers.ForEach(player => player.mute = !IsSoundOn);
    }

    public void PlayMusic(MusicType type)
    {
        int index = Array.FindIndex(musics, music => music.type == type);
        musicPlayer.clip = musics[index].audioClip;
        if (IsMusicOn)
            musicPlayer.Play(); 
    }

    public void PlaySFX(SFXType type)
    {
        if (IsSoundOn)
        {
            int index = Array.FindIndex(sounds, sound => sound.type == type);

            //AudioSource availableAudioSource = sfxPlayers.Find(player => !player.isPlaying) ?? gameObject.AddComponent<AudioSource>();
            AudioSource availableAudioSource = sfxPlayers.Find(player => !player.isPlaying);

            if(availableAudioSource == null && sfxPlayers.Count < maxSFXPlayers)
            {
                availableAudioSource = gameObject.AddComponent<AudioSource>();
                sfxPlayers.Add(availableAudioSource);
            }
                
            availableAudioSource.clip = sounds[index].audioClip;
            availableAudioSource.Play();
        }
    }

    public void ResetAudioSettings()
    {
        IsMusicOn = true;
        IsSoundOn = true;
        musicPlayer.mute = false;
        foreach (AudioSource source in sfxPlayers)
            source.mute = false;
    }
}
    
[Serializable]
public class Music
{
    public MusicType type;
    public AudioClip audioClip;
}

[Serializable]
public class Sound
{
    public SFXType type;
    public AudioClip audioClip;
}

public enum MusicType
{
    HomeMusic,
    GameMusic
}

public enum SFXType
{
    Click,
    Win,
    Loose
}
