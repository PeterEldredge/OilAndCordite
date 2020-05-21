using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioCue", menuName = "AudioCue", order = 1)]
public class AudioCue : ScriptableObject
{

    public string cueName;
    public AudioClip[] clips;

    [Range(0, 256)]
    public int priority = 128;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(-3f, 3f)]
    public float pitch = 1f;

    [Range(-1f, 1f)]
    public float stereoPan = 1f;

    [Range(0f, 1f)]
    public float spatialBlend = 0f;

    [Range(0f, 1.1f)]
    public float reverbZoneMix = 1f;

    [Range(0f, 5f)]
    public float dopplerLevel = 1f;

    [Range(0f, 50f)]
    public float minDistance = 1f;

    [Range(0f, 500f)]
    public float maxDistance = 20f;

    public bool loop = false;

    public bool isOneShot = false;

    public bool playOnAwake = false;
    public bool playRandom = false;

    [Tooltip("Decides how distance from the audio source affects the volume. For 3D Audio Only.")]public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;

    [HideInInspector]
    public AudioSource source;

}
