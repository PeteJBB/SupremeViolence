using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Jukebox : Singleton<Jukebox> 
{
    public MusicTrack[] Tracks;
    private AudioSource audioSource;

    [HideInInspector]
    public MusicTrack CurrentTrack;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 1;
    }

    public void PlayMomemt(MusicMoments moment)
    {
        if (CurrentTrack != null && (CurrentTrack.Usages & moment) == moment)
            return; // already playing a track that fits this

        // look for a track that suits the given moment
        var tracks = Tracks.Where(x => (x.Usages & moment) == moment).ToList();
        if (!tracks.Any())
        {
            Debug.LogError("No music track for moment " + moment);
            return;
        }

        var chosenTrack = tracks[Random.Range(0, tracks.Count)];

        // fade out, switch track and fade in
        iTween.AudioTo(gameObject, iTween.Hash("volume", 0, "time", 1, "oncomplete", (System.Action)(() =>
        {
            audioSource.clip = chosenTrack.Clip;
            audioSource.loop = true;
            audioSource.Play();

            iTween.AudioTo(gameObject, 1, 1, 1);
        })));

        CurrentTrack = chosenTrack;
    }

    public void Stop()
    {
        iTween.AudioTo(gameObject, iTween.Hash("volume", 0, "time", 1, "oncomplete", (System.Action)(() =>
        {
            audioSource.Stop();
        })));
    }
}

[System.Serializable]
public class MusicTrack
{
    public AudioClip Clip;

    [SerializeField]
    [EnumFlags]
    public MusicMoments Usages;
}

[System.Flags]
public enum MusicMoments
{
    TitleScreen = 1,
    Menu = 2,
    GameOn = 4
}

