using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _startScreenParent;
    [SerializeField] private GameObject _gameoverScreenParent;

    public void OnStartButtonPressed()
    {
        StartGame();
    }

    void StartGame()
    {
        EventManager.TriggerEvent(Constants.Events.GAME_START);
        ShowGameScreen();
    }

    void ShowStartScreen()
    {
        _startScreenParent.SetActive(true);
        //_gameoverScreenParent.SetActive(false);
    }

    void ShowGameScreen()
    {
        _startScreenParent.SetActive(false);
        //_gameoverScreenParent.SetActive(false);
    }

    void ShowGameOverScreen()
    {
        //_gameoverScreenParent.SetActive(true);
        _startScreenParent.SetActive(false);
    }
    
    
    private void Awake()
    {
        ShowStartScreen();
    }

    private void OnEnable()
    {
        EventManager.StartListening(Constants.Events.PLAYER_DESTROYED, OnPlayerDestroyed);
        EventManager.StartListening(Constants.Events.ALL_ENEMIES_DESTROYED, OnAllEnemiesDestroyed);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Constants.Events.PLAYER_DESTROYED, OnPlayerDestroyed);
        EventManager.StartListening(Constants.Events.ALL_ENEMIES_DESTROYED, OnAllEnemiesDestroyed);
    }

    void OnPlayerDestroyed()
    {
        Debug.Log("Game over!");   
        EventManager.TriggerEvent(Constants.Events.GAME_OVER, false);
        //ShowGameOverScreen();
        ShowStartScreen();
    }

    void OnAllEnemiesDestroyed()
    {
        Debug.Log("You win this round!");
        EventManager.TriggerEvent(Constants.Events.GAME_OVER, true);
    }
}
