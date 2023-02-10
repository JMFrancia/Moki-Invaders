using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOnDisable : MonoBehaviour
{
    [SerializeField] GameObject _explosionPrefab;

    private void OnDisable()
    {
        if(!gameObject.scene.isLoaded) 
            return;
        
        GameObject explosion = ObjectPoolManager.Get(_explosionPrefab, true);
        explosion.transform.position = transform.position;
        explosion.transform.parent = null;
    }
}
