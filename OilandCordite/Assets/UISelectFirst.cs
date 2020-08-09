using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelectFirst : MonoBehaviour
{
    void Start()
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
