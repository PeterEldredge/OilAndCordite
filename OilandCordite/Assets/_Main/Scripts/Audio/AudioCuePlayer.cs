using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCuePlayer : MonoBehaviour
{

    [SerializeField] private AudioCue[] _audioCues;

    private Dictionary<string, AudioSource> _audioSourceDictionary;
    
    // Start is called before the first frame update
    void Awake()
    {
        _audioSourceDictionary = new Dictionary<string, AudioSource>();

        foreach(AudioCue cue in _audioCues)
        {
            _audioSourceDictionary.Add(cue.cueName, gameObject.AddComponent<AudioSource>());

            _audioSourceDictionary[cue.cueName] = gameObject.AddComponent<AudioSource>();

            _audioSourceDictionary[cue.cueName].clip = cue.clips[0];
            _audioSourceDictionary[cue.cueName].priority = cue.priority;
            _audioSourceDictionary[cue.cueName].volume = cue.volume;
            _audioSourceDictionary[cue.cueName].pitch = cue.pitch;
            _audioSourceDictionary[cue.cueName].panStereo = cue.stereoPan;
            _audioSourceDictionary[cue.cueName].spatialBlend = cue.spatialBlend;
            _audioSourceDictionary[cue.cueName].minDistance = cue.minDistance;
            _audioSourceDictionary[cue.cueName].maxDistance = cue.maxDistance;
            _audioSourceDictionary[cue.cueName].rolloffMode = cue.rolloffMode;
            _audioSourceDictionary[cue.cueName].playOnAwake = cue.playOnAwake;
            _audioSourceDictionary[cue.cueName].ignoreListenerPause = cue.ignoreListenerPause;
            _audioSourceDictionary[cue.cueName].loop = cue.loop;

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
                _audioSourceDictionary[cue.cueName].PlayOneShot(cue.clips[audioIndex]);
            }
            else
            {
                _audioSourceDictionary[cue.cueName].Play();
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
                _audioSourceDictionary[cue.cueName].PlayOneShot(cue.clips[UnityEngine.Random.Range(0, cue.clips.Length)]);
            }
            else
            {
                _audioSourceDictionary[cue.cueName].Play();
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
            _audioSourceDictionary[cue.cueName].Stop();
        }
    }
}
