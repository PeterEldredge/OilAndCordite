using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyDetectUI : MonoBehaviour
{
    private int frameCount=0;
    [SerializeField] private AnimationCurve _heatCurve;
    [SerializeField] private int _maxSize;
    void Start()
    {
        frameCount = 0;
    }
    void FixedUpdate()
    {
        if (frameCount % 4 == 0)
        {
            //Resize(Vector3.Distance(PlayerData.Instance.transform.position,transform.position));
        }
        
    }
    private void Resize(float distance)
    {
        float size = (_heatCurve.Evaluate(distance) * _maxSize)/transform.parent.localScale.x;
        transform.localScale = new Vector3(size, size, size);
    }
}
