using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCuePlayer : MonoBehaviour
{

    [SerializeField] private AudioCue[] _audioCues;
    
    // Start is called before the first frame update
    void Start()
    {
        foreach(AudioCue cue in _audioCues)
        {
            cue.source = gameObject.AddComponent<AudioSource>();
            cue.source.clip = cue.clips[0];
            cue.source.priority = cue.priority;
            cue.source.volume = cue.volume;
            cue.source.pitch = cue.pitch;
            cue.source.panStereo = cue.stereoPan;

            cue.source.loop = cue.loop;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
