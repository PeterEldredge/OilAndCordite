using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCuePlayer : MonoBehaviour
{

    [SerializeField] private AudioCue[] _audioCues;
    
    // Start is called before the first frame update
    void Awake()
    {
        foreach(AudioCue cue in _audioCues)
        {
            cue.source = gameObject.AddComponent<AudioSource>();

            cue.source.clip = cue.clips[0];
            cue.source.priority = cue.priority;
            cue.source.volume = cue.volume;
            cue.source.pitch = cue.pitch;
            cue.source.panStereo = cue.stereoPan;
            cue.source.spatialBlend = cue.spatialBlend;
            //cue.source.dopplerLevel = cue.dopplerLevel;
            cue.source.minDistance = cue.minDistance;
            cue.source.maxDistance = cue.maxDistance;
            cue.source.rolloffMode = cue.rolloffMode;
            cue.source.playOnAwake = cue.playOnAwake;

            cue.source.loop = cue.loop;
            if (cue.playOnAwake)
            {
                if (cue.playRandom)
                {
                    PlayRandomSound(cue.cueName);
                }
                else
                {
                    PlaySound(cue.cueName);
                }
            }
        }
    }

    public void PlaySound(string cueName, int audioIndex = 0)
    {
        AudioCue cue = Array.Find(_audioCues, cues => cues.cueName == cueName);
        if (cue == null)
        {
            Debug.Log(cueName + " does not exist.");
        }
        else
        {
            if (cue.isOneShot)
            {
                cue.source.PlayOneShot(cue.clips[audioIndex]);
            }
            else
            {
                cue.source.Play();
            }
        }
    }

    public void PlayRandomSound(string cueName)
    {
        AudioCue cue = Array.Find(_audioCues, cues => cues.cueName == cueName);
        if (cue == null)
        {
            Debug.Log(cueName + " does not exist.");
        }
        else
        {
            if (cue.isOneShot)
            {
                cue.source.PlayOneShot(cue.clips[UnityEngine.Random.Range(0, cue.clips.Length)]);
            }
            else
            {
                cue.source.Play();
            }
        }
    }

    public void StopSound(string cueName, int audioIndex)
    {
        AudioCue cue = Array.Find(_audioCues, cues => cues.cueName == cueName);
        if (cue == null)
        {
            Debug.Log(cueName + "does not exist.");
        }
        else
        {
            cue.source.Stop();
        }
    }
}
