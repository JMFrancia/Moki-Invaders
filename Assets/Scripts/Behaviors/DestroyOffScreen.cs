using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOffScreen : MonoBehaviour
{
    [SerializeField] private bool _startOffscreen = false;
    
    private Vector3 _minPos;
    private Vector3 _maxPos;
    private Vector3 _spriteSize;
    private bool _active;

    private void Awake()
    {
        _spriteSize = GetComponent<Renderer>().bounds.size;
        _minPos = Camera.main.ViewportToWorldPoint(Vector3.zero) - _spriteSize / 2;
        _maxPos = Camera.main.ViewportToWorldPoint(Vector3.one) + _spriteSize / 2;

        if (_startOffscreen)
        {
            _active = false;
        }
    }

    private void Update()
    {
        if (_startOffscreen && !_active)
        {
            _active = !IsOffScreen();
            return;
        }

        if (IsOffScreen())
        {
            ObjectPoolManager.Release(gameObject, true);
        }
    }

    bool IsOffScreen()
    {
        Vector3 pos = transform.position; //loading into local memory for faster access
        return pos.x > _maxPos.x ||
               pos.y > _maxPos.y ||
               pos.x < _minPos.x ||
               pos.y < _minPos.y;
    }
}
