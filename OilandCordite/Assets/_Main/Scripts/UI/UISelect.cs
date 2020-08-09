using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelect : MonoBehaviour
{
    [SerializeField] private GameObject _selectUI;

    public void Select()
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(_selectUI);
    }
}
