using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleActiveSelf : MonoBehaviour
{
    public void toggleActive()
    {
        if (this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            this.gameObject.SetActive(true);
        }
    }
}
