using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionControllerData : MonoBehaviour
{
    public static MissionControllerData Instance { get; private set; }
    public MissionController MissionController { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        MissionController = GetComponent<MissionController>();
    }

}
