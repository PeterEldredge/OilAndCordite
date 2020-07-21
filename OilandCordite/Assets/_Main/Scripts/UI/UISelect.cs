using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelect : MonoBehaviour
{
    [SerializeField]private GameObject _selectUI;

    void Start()
    {
        SelectFirst();
    }

    public void SelectFirst()
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(_selectUI);
    }
}
