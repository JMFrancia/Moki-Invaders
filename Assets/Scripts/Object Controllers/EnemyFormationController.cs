using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/*
 * Controller class for the enemy formation.
 * As enemy count decreases, enemies will speed up and shoot more frequently.
 */
public class EnemyFormationController : MonoBehaviour
{
    [FormerlySerializedAs("_minShotSpeed")]
    [Tooltip("How frequently the enemy will shoot when the formation is full")]
    [SerializeField] private float _maxShotTime = 1.5f;
    [FormerlySerializedAs("_maxShotSpeed")]
    [Tooltip("How frequently the enemy will shoot when the formation is almost empty")]
    [SerializeField] private float _minShotTime = .04f;
    [Tooltip("How frequently the enemy will move when the formation is full")]
    [FormerlySerializedAs("_minStepSpeed")] [SerializeField] private float _maxStepTime = 1.5f;
    [Tooltip("How frequently the enemy will move when the formation is almost empty")]
    [FormerlySerializedAs("_maxStepSpeed")] [SerializeField] private float _minStepTime = .15f;
    [Tooltip("Speed multiplier for the last surviving enemy")]
    [SerializeField] private float _lastEnemySpeedMultiplier = 2f;
    [Tooltip("Horizontal step distance for the formation")]
    [SerializeField] private float _stepHorizontalDistance = .5f;
    [Tooltip("Vertical step distance for the formation")]
    [SerializeField] private float _stepVerticalDistance = .5f;
    [Tooltip("If true, will show a boundary gizmo at either side of the formation")]
    [SerializeField] private bool _debugShowBoundaryGizmos = false;

    private int TotalColumns => transform.childCount;

    private float _timeSinceLastMove;
    private float _timeSinceLastShot;
    private List<EnemyColumn> _activeColumns;
    private EnemyColumn[] _orderedColumns;
    private bool _movingLeft;
    private int _rightMostActiveColumn;
    private int _leftMostActiveColumn;
    private float _halfEnemySpriteWidth;
    private bool _active;
    private float _stepSpeed;
    private float _shotSpeed;
    private int _enemiesDestroyed;
    private int _totalEnemies;
    private Vector3 _originalPos;

    private float _rightScreenEdge;
    private float _leftScreenEdge;

    private void OnEnable()
    {
        EventManager.StartListening(Constants.Events.GAME_START, OnGameStart);
        EventManager.StartListening(Constants.Events.GAME_OVER, OnGameOver);
        EventManager.StartListening(Constants.Events.ENEMY_DESTROYED, OnEnemyDestroyed);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Constants.Events.GAME_START, OnGameStart);
        EventManager.StopListening(Constants.Events.GAME_OVER, OnGameOver);
        EventManager.StopListening(Constants.Events.ENEMY_DESTROYED, OnEnemyDestroyed);
    }

    private void Awake()
    {
        _orderedColumns = new EnemyColumn[TotalColumns];
        for (int n = 0; n < TotalColumns; n++)
        {
            _orderedColumns[n] = transform.GetChild(n).GetComponent<EnemyColumn>();
            _orderedColumns[n].ColumnDestroyed += OnColumnDestroyed;
        }

        //Assumes all columns are identical in size
        _totalEnemies = _orderedColumns[0].EnemyCount * _orderedColumns.Length;

        //TODO: Better way to get this than at runtime via multiple component searches?
        _halfEnemySpriteWidth =
            GetComponentInChildren<EnemyController>().GetComponent<SpriteRenderer>().bounds.size.x * .5f;

        _originalPos = transform.position;
        _active = false;

        _rightScreenEdge = Camera.main.ViewportToWorldPoint(Vector3.one).x;
        _leftScreenEdge = Camera.main.ViewportToWorldPoint(Vector3.zero).x;
    }

    private void Update()
    {
        if (!_active)
            return;

        UpdateShot();
    }
    
    private void FixedUpdate()
    {
        if (!_active)
            return;

        UpdateMove();
    }
    
    void OnGameStart()
    {
        ResetFormationAndActivate();
    }
    
    private void OnDrawGizmos()
    {
        if (_debugShowBoundaryGizmos && _active)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(new Vector3(GetLeftBoundaryEdge(), transform.position.y, transform.position.z),
                Vector3.one * _halfEnemySpriteWidth * 2);
            Gizmos.DrawCube(new Vector3(GetRightBoundaryEdge(), transform.position.y, transform.position.z),
                Vector3.one * _halfEnemySpriteWidth * 2);
        }
    }

    void OnEnemyDestroyed()
    {
        _enemiesDestroyed++;
        if ((_totalEnemies - _enemiesDestroyed) == 1)
        {
            _stepSpeed = _minStepTime / _lastEnemySpeedMultiplier;
        }
        else
        {
            float inverseEnemiesDestroyedNormalized = 1 - ((float)_enemiesDestroyed / (float)_totalEnemies);
            _stepSpeed = Mathf.Lerp(_minStepTime, _maxStepTime, inverseEnemiesDestroyedNormalized);
            _shotSpeed = Mathf.Lerp(_minShotTime, _maxShotTime, inverseEnemiesDestroyedNormalized);
        }
    }

    void OnGameOver(bool win)
    {
        _active = false;
    }

    void OnColumnDestroyed(EnemyColumn column)
    {
        _activeColumns.Remove(column);
        if (_activeColumns.Count == 0)
        {
            EventManager.TriggerEvent(Constants.Events.ALL_ENEMIES_DESTROYED);
            _active = false;
        }
        else
        {
            RecalculateColumnBoundaries();
        }
    }
    
    /*
     * Checks to see when an enemy should fire a shot and gives the order
     */
    void UpdateShot()
    {
        _timeSinceLastShot += Time.deltaTime;
        if (_timeSinceLastShot > _shotSpeed)
        {
            FireRandomShot();
            _timeSinceLastShot = 0;
        }
    }

    /*
     * Checks to see when the formation should move and gives the order
     */
    void UpdateMove()
    {
        _timeSinceLastMove += Time.deltaTime;
        if (_timeSinceLastMove > _stepSpeed)
        {
            Move();
            _timeSinceLastMove = 0;
        }
    }
    
    /*
     * Resets formation for new game
     */
    void ResetFormationAndActivate()
    {
        _timeSinceLastMove = 0f;
        _timeSinceLastShot = 0f;
        _movingLeft = false;
        ResetAllEnemies();
        _active = true;
        _stepSpeed = _maxStepTime;
        _shotSpeed = _maxShotTime;
        _enemiesDestroyed = 0;
        transform.position = _originalPos;
    }

    float GetLeftBoundaryEdge()
    {
        return _orderedColumns[_leftMostActiveColumn].transform.position.x - _halfEnemySpriteWidth;
    }

    float GetRightBoundaryEdge()
    {
        return _orderedColumns[_rightMostActiveColumn].transform.position.x + _halfEnemySpriteWidth;
    }

    /*
     * Recalculates which column should be considered the boundary on either side of the formation
     */
    void RecalculateColumnBoundaries()
    {
        for (; _leftMostActiveColumn < TotalColumns; _leftMostActiveColumn++)
        {
            if (!_orderedColumns[_leftMostActiveColumn].IsColumnEmpty())
            {
                break;
            }
        }

        for (; _rightMostActiveColumn >= 0; _rightMostActiveColumn--)
        {
            if (!_orderedColumns[_rightMostActiveColumn].IsColumnEmpty())
            {
                break;
            }
        }
    }

    /*
    * Fires a shot from a random bottom enemy
    */
    void FireRandomShot()
    {
        GetRandomActiveColumn()?.FireShot();
    }

    /*
    * Resets all enemies (IE restores them to life)
    */
    void ResetAllEnemies()
    {
        for (int n = 0; n < _orderedColumns.Length; n++)
        {
            _orderedColumns[n].ResetColumn();
        }

        _rightMostActiveColumn = _orderedColumns.Length - 1;
        _leftMostActiveColumn = 0;
        _activeColumns = new List<EnemyColumn>(_orderedColumns);
    }

    /*
     * Returns a random non-empty column
     */
    EnemyColumn GetRandomActiveColumn()
    {
        //Redundancy check
        if (_activeColumns.Count == 0)
            return null;

        int randomActiveColumnIndex = UnityEngine.Random.Range(0, _activeColumns.Count);
        return _activeColumns[randomActiveColumnIndex];
    }

    /*
    * Calculates next position for formation and moves it into that position
    */
    public void Move()
    {
        Vector3 delta = transform.position;
        if (_movingLeft)
        {
            if (Mathf.Approximately(_leftScreenEdge, GetLeftBoundaryEdge()))
            {
                _movingLeft = false;
                delta += new Vector3(0f, -1 * _stepVerticalDistance, 0f);
            }
            else
            {
                float deltaLeftBoundary = GetLeftBoundaryEdge() - _stepHorizontalDistance;
                float deltaLeft;
                if (deltaLeftBoundary > _leftScreenEdge)
                {
                    deltaLeft = _stepHorizontalDistance;
                }
                else
                {
                    deltaLeft = _stepHorizontalDistance - (_leftScreenEdge - deltaLeftBoundary);
                }

                delta += new Vector3(-1 * deltaLeft, 0f, 0f);
            }
        }
        else
        {
            if (Mathf.Approximately(_rightScreenEdge, GetRightBoundaryEdge()))
            {
                delta += new Vector3(0f, -1 * _stepVerticalDistance, 0f);
                _movingLeft = true;
            }
            else
            {
                float deltaRightBoundary = GetRightBoundaryEdge() + _stepHorizontalDistance;
                float deltaRight;
                if (deltaRightBoundary < _rightScreenEdge)
                {
                    deltaRight = _stepHorizontalDistance;
                }
                else
                {
                    deltaRight = _stepHorizontalDistance - (deltaRightBoundary - _rightScreenEdge);
                }

                delta += new Vector3(deltaRight, 0f, 0f);
            }
        }
        transform.position = delta;
    }
}