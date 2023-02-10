using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOnDisable : MonoBehaviour
{
    [SerializeField] private bool _setExplosionColorToMatch = true;
    [SerializeField] GameObject _explosionPrefab;

    public void Explode()
    {
        GameObject explosion = ObjectPoolManager.Get(_explosionPrefab, true);
        explosion.transform.position = transform.position;
        explosion.transform.parent = null;

        if (_setExplosionColorToMatch)
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            if (renderer == null)
                return;

            explosion.GetComponent<SpriteRenderer>().color = renderer.color;
        }
    }

    private void OnDisable()
    {
        if(!gameObject.scene.isLoaded) 
            return;
        
        Explode();
    }
}
