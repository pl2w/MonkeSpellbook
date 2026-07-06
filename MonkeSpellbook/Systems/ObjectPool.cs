using System.Collections.Generic;
using UnityEngine;

namespace MonkeSpellbook.Systems;

public class ObjectPool
{
    private readonly GameObject _prefab;
    private readonly Queue<GameObject> _pool = new();
    private readonly Transform _parent;

    public ObjectPool(GameObject prefab, int prewarmCount, Transform parent = null)
    {
        _prefab = prefab;
        _parent = parent;

        for (var i = 0; i < prewarmCount; i++)
        {
            var obj = Object.Instantiate(_prefab, _parent);
            obj.SetActive(false);
            _pool.Enqueue(obj);
        }
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        var obj = _pool.Count > 0 ? _pool.Dequeue() : Object.Instantiate(_prefab, _parent);
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        return obj;
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        _pool.Enqueue(obj);
    }
}