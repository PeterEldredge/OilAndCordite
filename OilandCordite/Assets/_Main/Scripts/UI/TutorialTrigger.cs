using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : GameEventUserObject
{
    [SerializeField] private string _text;
    // Start is called before the first frame update
    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag(Tags.PLAYER))
        {
            EventManager.Instance.TriggerEvent(new Events.TutorialArgs(_text));
            gameObject.SetActive(false);
        }
    }
}
