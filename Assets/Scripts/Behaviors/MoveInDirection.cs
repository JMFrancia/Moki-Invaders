using UnityEngine;

/*
 * Simple script to move transform in a cardinal direction at given speed.
 */
public class MoveInDirection : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private Direction _direction;
    [Tooltip("If true, will attempt to use the object's rigidbody for motion")]
    [SerializeField] private bool _useRigidBodyIfAvailable = false;

    private Rigidbody2D _rigidbody2D; 
    
    [System.Serializable]
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void SetDirection(Direction dir)
    {
        _direction = dir;
    }

    private void FixedUpdate()
    {
        MoveDirection(_direction, _speed);
    }

    void MoveDirection(Direction dir, float amt)
    {
        Vector3 delta = Vector3.zero;
        switch (dir)
        {
            case Direction.Up:
                delta = Vector3.up;
                break;
            case Direction.Down:
                delta = Vector3.down;
                break;
            case Direction.Left:
                delta = Vector3.left;
                break;
            case Direction.Right:
                delta = Vector3.right;
                break;
        }

        Vector3 newPos = transform.position + delta * amt;
        
        if (_useRigidBodyIfAvailable && _rigidbody2D != null)
        {
            _rigidbody2D.MovePosition(newPos);
        }
        else
        {
            transform.position = newPos;
        }
    }
}
