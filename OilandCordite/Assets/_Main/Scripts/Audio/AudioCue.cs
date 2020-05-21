using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioCue", menuName = "AudioCue", order = 1)]
public class AudioCue : ScriptableObject
{

    public AudioClip[] clips;

    [Range(0, 256)]
    public int priority = 128;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(-3f, 3f)]
    public float pitch = 1f;

    [Range(-1f, 1f)]
    public float stereoPan = 0f;

    [Range(0f, 1f)]
    public float spatialBlend = 0f;

    [Range(0f, 1.1f)]
    public float reverbZoneMix = 1f;

    public bool loop;

    public bool isOneShot;

    [HideInInspector]
    public AudioSource source;

}
