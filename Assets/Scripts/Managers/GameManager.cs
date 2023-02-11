using UnityEngine;
using UnityEngine.UI;

/*
 * Game manager is charge of the overall game state; in this case, which "screen" is showing,
 * and when we are in game over.
 */
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _startScreenParent;
    [SerializeField] private GameObject _gameoverScreenParent;
    [SerializeField] private Text _gameoverText;

    //TODO: Move these to external XML or JSON along with other user-facing text for easy localization
    private const string GAME_OVER_LOSS_MSG = "Game Over";
    private const string GAME_OVER_WIN_MSG = "You Win!!!";
    
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
        _gameoverScreenParent.SetActive(false);
    }

    void ShowGameScreen()
    {
        _startScreenParent.SetActive(false);
        _gameoverScreenParent.SetActive(false);
    }

    void ShowGameOverScreen(bool win)
    {
        _gameoverScreenParent.SetActive(true);
        _startScreenParent.SetActive(false);
        _gameoverText.text = win ? GAME_OVER_WIN_MSG : GAME_OVER_LOSS_MSG;
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
        EventManager.TriggerEvent(Constants.Events.GAME_OVER, false);
        ShowGameOverScreen(false);
    }

    void OnAllEnemiesDestroyed()
    {
        EventManager.TriggerEvent(Constants.Events.GAME_OVER, true);
        ShowGameOverScreen(true);
    }
}
