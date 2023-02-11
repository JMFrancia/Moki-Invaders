using UnityEngine;

/*
 * Destroys game object if it goes offscreen.
 * Includes options for starting offscreen, and for adding buffer zone.
 */
public class DestroyOffScreen : MonoBehaviour
{
    [Tooltip("If true, will start checking if off-screen until object is on screen")]
    [SerializeField] private bool _startOffscreen = false;
    [Tooltip("Buffer area around game object, for dealing with large or slow sprites")]
    [SerializeField] private float _buffer = 2f;
    
    private Vector3 _minPos;
    private Vector3 _maxPos;
    private Vector3 _spriteSize;
    private bool _active;

    private void Awake()
    {
        _spriteSize = GetComponent<Renderer>().bounds.size;
        _minPos = Camera.main.ViewportToWorldPoint(Vector3.zero) - _spriteSize / 2;
        _maxPos = Camera.main.ViewportToWorldPoint(Vector3.one) + _spriteSize / 2;

        if (_startOffscreen)
        {
            _active = IsOffScreen();
        }
    }

    private void Update()
    {
        if (_startOffscreen && !_active)
        {
            _active = !IsOffScreen();
            return;
        }

        if (IsOffScreen())
        {
            ObjectPoolManager.Release(gameObject, true);
        }
    }
    
    bool IsOffScreen()
    {
        Vector3 pos = transform.position; //loading into local memory for faster access
        return pos.x > _maxPos.x + _buffer ||
               pos.y > _maxPos.y + _buffer ||
               pos.x < _minPos.x - _buffer ||
               pos.y < _minPos.y - _buffer;
    }
}
