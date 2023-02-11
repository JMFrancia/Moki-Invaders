using UnityEngine;

/*
 * Generates the UFO within a specified random space and time
 */
public class UFOGenerator : MonoBehaviour
{
    [Tooltip("Controls the height range from which the UFO may randomly be launched")]
    [SerializeField] private float _possibleLaunchHeight = 1f;
    [Tooltip("The minimum amount of time before a UFO may appear")]
    [SerializeField] private float _minUFOTime = 8f;
    [Tooltip("The maximum amount of time before a UFO may appear")]
    [SerializeField] private float _maxUFOTime = 20f;
    [SerializeField] private GameObject _ufoPrefab;
    [Tooltip("Draws (as a gizmo) the height range from which the UFO may randomly be launched")]
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

    void OnGameStart()
    {
        ResetUFOTime();
        _active = true;
    }

    void OnGameOver(bool win)
    {
        _active = false;
    }

    void ResetUFOTime()
    {
        _timeSinceLastUFO = 0f;
        _timeUntilNextUFO = Random.Range(_minUFOTime, _maxUFOTime);
    }

    /*
     * Checks when to generate UFO, and gives order to do so
     */
    void CheckToLaunchUFO()
    {
        _timeSinceLastUFO += Time.deltaTime;
        if (_timeSinceLastUFO > _timeUntilNextUFO)
        {
            LaunchUFO();
            ResetUFOTime();
        }
    }

    /*
     * Generates the UFO at random height within launch height of the generator's position
     */
    void LaunchUFO()
    {
        float launchHeight = transform.position.y +
                             Random.Range(-.5f * _possibleLaunchHeight, .5f * _possibleLaunchHeight);
        Vector3 launchPosition = new Vector3(transform.position.x, launchHeight, transform.position.z);
        GameObject UFO = ObjectPoolManager.Get(_ufoPrefab, true);
        UFO.transform.position = launchPosition;
        EventManager.TriggerEvent(Constants.Events.UFO_LAUNCHED);
    }
}
