using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class UFOGenerator : MonoBehaviour
{
    [SerializeField] private float _possibleLaunchHeight = 1f;
    [SerializeField] private float _minUFOTime = 8f;
    [SerializeField] private float _maxUFOTime = 20f;
    [SerializeField] private GameObject _ufoPrefab;
    [SerializeField] private bool _debugDrawLaunchRangeGizmos = true;

    private float _timeSinceLastUFO;
    private float _timeUntilNextUFO;

    private bool _active;
    
    private void OnEnable()
    {
        EventManager.StartListening(Constants.Events.GAME_START, OnGameStart);
        EventManager.StartListening(Constants.Events.GAME_OVER, OnGameOver);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Constants.Events.GAME_START, OnGameStart);
        EventManager.StartListening(Constants.Events.GAME_OVER, OnGameOver);
    }

    void OnGameStart()
    {
        ResetUFOTime();
        _active = true;
    }

    void OnGameOver(bool win)
    {
        _active = false;
    }

    void Update()
    {
        if (!_active)
            return;
        
        CheckToLaunchUFO();
    }

    private void OnDrawGizmos()
    {
        if (_debugDrawLaunchRangeGizmos)
        {
            Vector3 _gizmoCubeSize = new Vector3(.25f, _possibleLaunchHeight, .25f);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, _gizmoCubeSize);
        }
    }

    void ResetUFOTime()
    {
        _timeSinceLastUFO = 0f;
        _timeUntilNextUFO = UnityEngine.Random.Range(_minUFOTime, _maxUFOTime);
    }

    void CheckToLaunchUFO()
    {
        _timeSinceLastUFO += Time.deltaTime;
        if (_timeSinceLastUFO > _timeUntilNextUFO)
        {
            LaunchUFO();
            ResetUFOTime();
        }
    }

    void LaunchUFO()
    {
        float launchHeight = transform.position.y +
                             UnityEngine.Random.Range(-.5f * _possibleLaunchHeight, .5f * _possibleLaunchHeight);
        Vector3 launchPosition = new Vector3(transform.position.x, launchHeight, transform.position.z);
        Instantiate(_ufoPrefab, launchPosition, Quaternion.identity);
        EventManager.TriggerEvent(Constants.Events.UFO_LAUNCHED);
    }
}
