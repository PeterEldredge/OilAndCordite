using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFirstSelect : MonoBehaviour
{
    [SerializeField]private GameObject _FirstSelect;

    void Start()
    {
        SelectFirst();
    }

    public void SelectFirst()
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(_FirstSelect);
    }
}
