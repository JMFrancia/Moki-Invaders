using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/*
 * Object pooling manager, using Unity Engine's built in pooling system.
 * For each poolable prefab, be sure to attach PoolableObject component.
 */
public class ObjectPoolManager : MonoBehaviour
{
    [Serializable]
    public struct ObjectPoolData {
        public GameObject poolObject;
        public PoolableObject.PoolableType type;
        public int count;
        public bool expandable;
    }

    [SerializeField] private bool _showErrorMessages = true;
    [SerializeField] List<ObjectPoolData> _poolData;

    List<ObjectPool<GameObject>> _pools;
    Dictionary<PoolableObject.PoolableType, ObjectPool<GameObject>> _poolDict;

    const string GET_FAIL_STRING = "Attempt to call ObjectPoolManager.Get() with non-pooled object {0}.";
    const string RELEASE_FAIL_STRING = "Attempt to call ObjectPoolManager.Release() with non-pooled object {0}.";

    static ObjectPoolManager _instance;

    public static GameObject Get(GameObject obj, bool instantiateOnFail = false)
    {
        return _instance.GetInternal(obj, instantiateOnFail);
    }

    public static bool Release(GameObject obj, bool destroyOnFail = false) {
        return _instance.ReleaseInternal(obj, destroyOnFail);
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else {
            Destroy(_instance);
        }
    }

    ObjectPool<GameObject> GeneratePool(ObjectPoolData data) {
        return new ObjectPool<GameObject>(
            createFunc: () => Instantiate(data.poolObject, Vector3.zero, Quaternion.identity),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: true,
            defaultCapacity: data.count,
            maxSize: data.expandable ? 10000 : data.count
        );
    }

    private void Start()
    {
        GenerateObjectPools(_poolData);
    }

    GameObject GetInternal(GameObject obj, bool instantiateOnFail) {
        GameObject result = null;
        ObjectPool<GameObject> pool;
        PoolableObject po = obj.GetComponent<PoolableObject>();
        if (po == null)
        {
            LogError($"No PoolableObject component found on {obj}");
        }
        else {
            if (_poolDict.TryGetValue(po.Type, out pool))
            {
                result = pool.Get();
            }
            else
            {
                string failMsg = GET_FAIL_STRING;
                if (instantiateOnFail)
                {
                    result = Instantiate(obj, Vector3.zero, Quaternion.identity);
                    failMsg += " Instantiating new.";
                }
                LogError(string.Format(failMsg, po.Type));
            }
        }
        return result;
    }

    bool ReleaseInternal(GameObject obj, bool destroyOnFail) {
        ObjectPool<GameObject> pool;
        bool result = false;

        PoolableObject po = obj.GetComponent<PoolableObject>();
        if (po == null)
        {
            if (destroyOnFail)
            {
                Destroy(obj);
            }
            LogError($"No PoolableObject component found on {obj}");
        }
        else {
            if (_poolDict.TryGetValue(po.Type, out pool))
            {
                try
                {
                    pool.Release(obj);
                }
                catch (InvalidOperationException e)
                {
                    if (_showErrorMessages)
                    {
                        LogError(e.Message);
                    }
                }
                result = true;
            }
            else
            {
                string failMsg = RELEASE_FAIL_STRING;
                if (destroyOnFail)
                {
                    Destroy(obj);
                    failMsg += " Destroying instance.";
                }
                LogError(string.Format(failMsg, po.Type));
            }
        }
        return result;
    }

    void LogError(string msg)
    {
        if (_showErrorMessages)
        {
            Debug.LogError(msg);
        }
    }

    void GenerateObjectPools(List<ObjectPoolData> data) {
        if (_pools == null)
        {
            _pools = new List<ObjectPool<GameObject>>();
        }
        else {
            _pools.Clear();
        }

        if (_poolDict == null)
        {
            _poolDict = new Dictionary<PoolableObject.PoolableType, ObjectPool<GameObject>>();
        }
        else {
            _poolDict.Clear();
        }

        foreach (ObjectPoolData datum in data) {
            ObjectPool<GameObject> pool = GeneratePool(datum);
            _poolDict.Add(datum.poolObject.GetComponent<PoolableObject>().Type, pool);
            _pools.Add(pool);
        }
    }
}
