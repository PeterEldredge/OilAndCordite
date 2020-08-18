using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingSettigs : MonoBehaviour
{
    private PostProcessProfile _postProcessProfile;

    private void Awake()
    {
        _postProcessProfile = GetComponent<PostProcessVolume>().sharedProfile;
    }

    private void Start()
    {
        if(Settings.Instance)
        {
            if (_postProcessProfile.TryGetSettings(out MotionBlur motionBlur))
            {
                motionBlur.active = Settings.Instance.MotionBlur;
            }
        }
    }
}
