using UnityEngine;

[RequireComponent(typeof(FireShot))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _shotRate = 1f;

    private Rigidbody2D _rigidbody2D;
    private Renderer _renderer;
    private FireShot _fireShot;
    
    private float _minPosX;
    private float _maxPosX;
    private float _timeSinceLastShot = 0f;
    private bool _active;

    private void OnEnable()
    {
        EventManager.StartListening(Constants.Events.GAME_START, OnGameStart);
    }
    
    private void OnDisable()
    {
        EventManager.StopListening(Constants.Events.GAME_START, OnGameStart);
    }

    void OnGameStart()
    {
        Reset();
    }

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<Renderer>();
        _fireShot = GetComponent<FireShot>();
        
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(Constants.Layers.ENEMY_SHOT) && other.gameObject.activeInHierarchy)
        {
           Die();
           ObjectPoolManager.Release(other.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer(Constants.Layers.ENEMY) && col.gameObject.activeInHierarchy)
        {
            Die();
        }
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
        EventManager.TriggerEvent(Constants.Events.PLAYER_DESTROYED);
    }

    void CheckToFireShot()
    {
        //Choosing to do it this way instead of using a coroutine to reduce overhead
        _timeSinceLastShot += Time.deltaTime;
        if (_timeSinceLastShot >= _shotRate)
        {
            _fireShot.Fire();
            EventManager.TriggerEvent(Constants.Events.PLAYER_SHOT_FIRED);
            _timeSinceLastShot = 0f;
        }
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
        pos = new Vector3(Mathf.Clamp(pos.x, _minPosX, _maxPosX), transform.position.y, transform.position.z);
        _rigidbody2D.MovePosition(pos);
    }
}
