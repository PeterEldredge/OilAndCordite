using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFirstSelect : MonoBehaviour
{
    [SerializeField]private GameObject _FirstSelect;
    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(_FirstSelect);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
