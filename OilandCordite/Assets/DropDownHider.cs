using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Lets just not talk about this nonsense
public class DropDownHider : MonoBehaviour
{
    private HideOnDropDown[] _objectsToHide;

    private void Start()
    {
        if(_objectsToHide == null) _objectsToHide = FindObjectsOfType<HideOnDropDown>();
        
        foreach(HideOnDropDown objectToHide in _objectsToHide)
        {
            objectToHide.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (_objectsToHide == null) _objectsToHide = FindObjectsOfType<HideOnDropDown>();

        foreach (HideOnDropDown objectToHide in _objectsToHide)
        {
            objectToHide.gameObject.SetActive(true);
        }
    }
}
