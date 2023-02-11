using UnityEngine;
using UnityEngine.Serialization;

/*
 * Controller class for the player object
 */
[RequireComponent(typeof(FireShot), typeof(DieOnContact))]
public class PlayerController : MonoBehaviour
{
    [Tooltip("Time between automatic shots")]
    [FormerlySerializedAs("_shotRate")] [SerializeField] private float _shotTime = 1f;

    private Rigidbody2D _rigidbody2D;
    private Renderer _renderer;
    private FireShot _fireShot;
    private ExplodeOnDisable _exploder;
    
    private float _minPosX;
    private float _maxPosX;
    private float _timeSinceLastShot = 0f;
    private bool _active;
    
    private void OnEnable()
    {
        EventManager.StartListening(Constants.Events.GAME_START, OnGameStart);
        EventManager.StartListening(Constants.Events.GAME_OVER, OnGameOver);
    }
    
    private void OnDisable()
    {
        EventManager.StopListening(Constants.Events.GAME_START, OnGameStart);
        EventManager.StopListening(Constants.Events.GAME_OVER, OnGameOver);
    }
    
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<Renderer>();
        _fireShot = GetComponent<FireShot>();
        _exploder = GetComponent<ExplodeOnDisable>();
        GetComponent<DieOnContact>().OnDeath += Die;
        
        //TODO: Both this and Enemy Formation are doing calculations based on screen edges. Refactor those to common utility class.
        float spriteHalfWidth = GetComponent<Renderer>().bounds.size.x / 2f;
        _minPosX = Camera.main.ViewportToWorldPoint(Vector3.zero).x + spriteHalfWidth;
        _maxPosX = Camera.main.ViewportToWorldPoint(Vector3.one).x - spriteHalfWidth;
        _active = false;
    }
    
    private void Update()
    {
        if (!_active)
            return;
        
        CheckToFireShot();
    }

    private void FixedUpdate()
    {
        if (!_active)
            return;
        
        SetPosition(GetPlayerInput());
    }

    private void OnGameStart()
    {
        Reset();
    }

    private void OnGameOver(bool win)
    {
        _active = false;
    }

    void Reset()
    {
        _active = true;
        _renderer.enabled = true;
    }

    void Die()
    {
        _active = false;
        _renderer.enabled = false;
        _exploder.ForceExplode();
        EventManager.TriggerEvent(Constants.Events.PLAYER_DESTROYED);
    }

    /*
     * Checks when to fire shot and gives order to do so
     */
    void CheckToFireShot()
    {
        _timeSinceLastShot += Time.deltaTime;
        if (_timeSinceLastShot >= _shotTime)
        {
            _fireShot.Fire();
            EventManager.TriggerEvent(Constants.Events.PLAYER_SHOT_FIRED);
            _timeSinceLastShot = 0f;
        }
    }

    /*
     * Grabs control input from mouse or screen touch
     */
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
    
    /*
     * Sets player position, clamped to screen width
     */
    void SetPosition(Vector3 pos)
    {
        pos = new Vector3(Mathf.Clamp(pos.x, _minPosX, _maxPosX), transform.position.y, transform.position.z);
        _rigidbody2D.MovePosition(pos);
    }
}
