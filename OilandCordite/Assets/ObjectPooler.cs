using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType
{
    FastLaser,
    SlowLaser,
    MuzzleFlash,
}

[Serializable]
public struct PoolData
{
    [SerializeField] public ProjectileType Type;
    [SerializeField] public GameObject Projectile;
    [SerializeField] public int BaseProjectileCount;
}

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    [SerializeField] private List<PoolData> _poolData;

    private Dictionary<ProjectileType, Queue<GameObject>> _projectilePools;

    private void Awake()
    {
        if(!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        _projectilePools = new Dictionary<ProjectileType, Queue<GameObject>>();

        foreach(PoolData data in _poolData)
        {
            _projectilePools.Add(data.Type, new Queue<GameObject>());

            Transform poolHolder = new GameObject($"{Enum.GetName(typeof(ProjectileType), data.Type)}-Pool").transform;

            for(int i = 0; i < data.BaseProjectileCount; i++)
            {
                _projectilePools[data.Type].Enqueue(Instantiate(data.Projectile, poolHolder));
                _projectilePools[data.Type].Peek().SetActive(false);
            }
        }
    }

    public GameObject GetPooledObject(ProjectileType type)
    {
        GameObject temp = _projectilePools[type].Dequeue();

        _projectilePools[type].Enqueue(temp);

        return temp;
    }
}
