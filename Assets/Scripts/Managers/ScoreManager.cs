using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.Events;
using UnityEngine.UI;

/*
 * Score Manager keeps track of the player's score and updates the UI
 */
public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int _scorePerKill;
    [SerializeField] private int _scorePerUFO;
    [SerializeField] private Text _scoreText;

    private int _score;
    
    private void OnEnable()
    {
        EventManager.StartListening(Constants.Events.ENEMY_DESTROYED, OnEnemyDestroyed);
        EventManager.StartListening(Constants.Events.UFO_DESTROYED, OnUFODestroyed);
        EventManager.StartListening(Constants.Events.GAME_START, OnGameStart);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Constants.Events.ENEMY_DESTROYED, OnEnemyDestroyed);
        EventManager.StopListening(Constants.Events.UFO_DESTROYED, OnUFODestroyed);
        EventManager.StopListening(Constants.Events.GAME_START, OnGameStart);
    }

    void OnEnemyDestroyed()
    {
        _score += _scorePerKill;
        SetScoreText(_score);
    }

    void OnUFODestroyed()
    {
        _score += _scorePerUFO;
        SetScoreText(_score);
    }

    void OnGameStart()
    {
        _score = 0;
        SetScoreText(0);
    }

    void SetScoreText(int score)
    {
        _scoreText.text = _score.ToString("00000");
    }
}
