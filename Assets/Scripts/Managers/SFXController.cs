using System;
using UnityEngine;
using UnityEngine.Events;

/*
 * SFX Controller works with SFX Manager to activate audio clips according to in-game events
 */
[RequireComponent(typeof(SFXManager))]
public class SFXController : MonoBehaviour
{
    [SerializeField] private bool _showDebugMessages = false;
    [Tooltip("Switch SFX on/off")]
    [SerializeField] private bool _playSFX = true;
    [Tooltip("Switch music on/off")]
    [SerializeField] private bool _playMusic = true;
    [Tooltip("SFX volume")]
    [SerializeField] private float _sfxVolume = 1f;
    [Header("SFX")]
    [SerializeField] private AudioClip _gameStartSFX;
    [SerializeField] private AudioClip _enemyKillSFX;
    [SerializeField] private AudioClip _playerKillSFX;
    [SerializeField] private AudioClip _UFOKillSFX;
    [SerializeField] private AudioClip _playerShotSFX;
    [SerializeField] private AudioClip _enemyShotSFX;
    [SerializeField] private AudioClip _UFOSFX;
    [SerializeField] private AudioClip _victorySFX;
    [Header("Music")] 
    [Tooltip("Music volume")]
    [SerializeField] private float _musicVolume = 1f;
    [Tooltip("Time between notes at start")]
    [SerializeField] private float _minTempo = .5f;
    [Tooltip("Time between notes at end")]
    [SerializeField] private float _maxTempo = .1f;
    [SerializeField] private AudioClip[] _notes;

    private SFXManager _sfxManager;

    private Guid _musicSequenceInstanceID;

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
        EventManager.StartListening(Constants.Events.ENEMY_FORMATION_DECREASED, (UnityAction<float>) OnEnemyFormationDecreased);
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
        EventManager.StopListening(Constants.Events.ENEMY_FORMATION_DECREASED, (UnityAction<float>) OnEnemyFormationDecreased);
    }

    private void Awake()
    {
        _sfxManager = GetComponent<SFXManager>();
    }

    void OnGameStart()
    {
        PlaySFX(_gameStartSFX);
        _musicSequenceInstanceID = _sfxManager.PlayLoopingSequence(_notes, _minTempo, _musicVolume);
    }

    void OnEnemyDestroyed()
    {
        PlaySFX(_enemyKillSFX);
    }

    void OnEnemyFormationDecreased(float enemyPercentage)
    {
        if (!_playMusic)
            return;
        
        float inverseEnemiesDestroyedNormalized = 1 - enemyPercentage;
        float tempo = Mathf.Lerp(_maxTempo, _minTempo, inverseEnemiesDestroyedNormalized);
        _sfxManager.ChangeLoopingSequenceTempo(_musicSequenceInstanceID, tempo);
    }

    void OnPlayerShotFired()
    {
        PlaySFX(_playerShotSFX);
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
        _sfxManager.StopLoopingSequence(_musicSequenceInstanceID);
    }

    void PlaySFX(AudioClip sfx)
    {
        if (!_playSFX)
            return;
        
        if (sfx == null)
        {
            DisplayDebugMessage($"Tried to play sfx, but none assigned");
        }
        else
        {
            DisplayDebugMessage($"Playing sfx {sfx.name}");
            _sfxManager.PlaySFX(sfx, _sfxVolume);
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
