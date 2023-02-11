using UnityEngine;

/*
 * SFX Controller works with SFX Manager to activate audio clips according to in-game events
 */
[RequireComponent(typeof(SFXManager))]
public class SFXController : MonoBehaviour
{
    [SerializeField] private bool _showDebugMessages = false;
    [SerializeField] private AudioClip _gameStartSFX;
    [SerializeField] private AudioClip _enemyKillSFX;
    [SerializeField] private AudioClip _playerKillSFX;
    [SerializeField] private AudioClip _UFOKillSFX;
    [SerializeField] private AudioClip _playerShotSFX;
    [SerializeField] private AudioClip _enemyShotSFX;
    [SerializeField] private AudioClip _UFOSFX;
    [SerializeField] private AudioClip _victorySFX;

    private SFXManager _sfxManager;

    private void OnEnable()
    {
        EventManager.StartListening(Constants.Events.GAME_START, OnGameStart);
        EventManager.StartListening(Constants.Events.ENEMY_DESTROYED, OnEnemyDestroyed);
        EventManager.StartListening(Constants.Events.PLAYER_DESTROYED, OnPlayerDestroyed);
        EventManager.StartListening(Constants.Events.UFO_DESTROYED, OnUFODestroyed);
        EventManager.StartListening(Constants.Events.UFO_LAUNCHED, OnUFOLaunched);
        EventManager.StartListening(Constants.Events.PLAYER_SHOT_FIRED, OnPlayerShotFired);
        EventManager.StartListening(Constants.Events.ENEMY_SHOT_FIRED, OnEnemyShotFired);
        EventManager.StartListening(Constants.Events.GAME_OVER, OnGameOver);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Constants.Events.GAME_START, OnGameStart);
        EventManager.StopListening(Constants.Events.ENEMY_DESTROYED, OnEnemyDestroyed);
        EventManager.StopListening(Constants.Events.PLAYER_DESTROYED, OnPlayerDestroyed);
        EventManager.StopListening(Constants.Events.UFO_DESTROYED, OnUFODestroyed);
        EventManager.StopListening(Constants.Events.UFO_LAUNCHED, OnUFOLaunched);
        EventManager.StopListening(Constants.Events.PLAYER_SHOT_FIRED, OnPlayerShotFired);
        EventManager.StopListening(Constants.Events.ENEMY_SHOT_FIRED, OnEnemyShotFired);
        EventManager.StopListening(Constants.Events.GAME_OVER, OnGameOver);
    }

    private void Awake()
    {
        _sfxManager = GetComponent<SFXManager>();
    }

    void OnGameStart()
    {
        PlaySFX(_gameStartSFX);
    }

    void OnEnemyDestroyed()
    {
        PlaySFX(_enemyKillSFX);
    }

    void OnPlayerShotFired()
    {
        PlaySFX(_playerShotSFX, .5f);
    }

    void OnPlayerDestroyed()
    {
        PlaySFX(_playerKillSFX);
    }

    void OnEnemyShotFired()
    {
        PlaySFX(_enemyShotSFX);
    }

    void OnUFOLaunched()
    {
        PlaySFX(_UFOSFX);
    }

    void OnUFODestroyed()
    {
        PlaySFX(_UFOKillSFX);
    }

    void OnGameOver(bool win)
    {
        if (win)
        {
            PlaySFX(_victorySFX);
        }
    }

    void PlaySFX(AudioClip sfx, float volume = 1f)
    {
        if (sfx == null)
        {
            DisplayDebugMessage($"Tried to play sfx, but none assigned");
        }
        else
        {
            DisplayDebugMessage($"Playing sfx {sfx.name}");
            _sfxManager.PlaySFX(sfx, volume);
        }
    }

    void DisplayDebugMessage(string msg)
    {
        if (_showDebugMessages)
        {
            Debug.Log(msg);
        }
    }
}
