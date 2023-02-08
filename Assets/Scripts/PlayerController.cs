using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject _shotPrefab;
    [SerializeField] private float _shotRate = 1f;
    
    private float _minPosX;
    private float _maxPosX;
    private float _timeSinceLastShot = 0f;
    
    private void Awake()
    {
        float spriteHalfWidth = GetComponent<Renderer>().bounds.size.x / 2f;
        _minPosX = Camera.main.ViewportToWorldPoint(Vector3.zero).x + spriteHalfWidth;
        _maxPosX = Camera.main.ViewportToWorldPoint(Vector3.one).x - spriteHalfWidth;
    }

    private void Update()
    {
        SetPosition(GetPlayerInput());
        CheckToFireShot();
    }

    void CheckToFireShot()
    {
        //Choosing to do it this way instead of using a coroutine to reduce overhead
        _timeSinceLastShot += Time.deltaTime;
        if (_timeSinceLastShot >= _shotRate)
        {
            FireShot();
            _timeSinceLastShot = 0f;
        }
    }

    void FireShot()
    {
        Instantiate(_shotPrefab, transform.position, Quaternion.identity);
        EventManager.TriggerEvent(Constants.Events.PLAYER_SHOT_FIRED);
    }

    Vector3 GetPlayerInput()
    {
        Vector3 result = transform.position;
#if UNITY_EDITOR || PLATFORM_WEBGL
        result = Camera.main.ScreenToWorldPoint(Input.mousePosition);
#elif PLATFORM_IOS || PLATFORM_ANDROID
        if(Input.touches.Length > 0) {
            result = Camera.main.ScreenToWorldPoint(Input.touches[Input.touchCount - 1].position);
        }
#endif
        return result;
    }
    void SetPosition(Vector3 pos)
    {
        transform.position = new Vector3(Mathf.Clamp(pos.x, _minPosX, _maxPosX), transform.position.y, transform.position.z);
    }
}
