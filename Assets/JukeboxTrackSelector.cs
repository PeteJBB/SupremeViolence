using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class JukeboxTrackSelector : MonoBehaviour 
{
    public MusicMoments Moment;
    public bool SwitchTrackOnStart = true;

    void Start()
    {
        if (SwitchTrackOnStart)
            SwitchTrackNow();
    }

    public void SwitchTrackNow()
    {
        Jukebox.Instance.PlayMomemt(Moment);
    }
}
