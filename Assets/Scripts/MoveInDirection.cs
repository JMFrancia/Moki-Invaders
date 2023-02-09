using UnityEngine;

/*
 * Simple script to move transform in a cardinal direction at given speed.
 * Not designed for use with RB-based physics objects.
 */
public class MoveInDirection : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private Direction _direction;

    [System.Serializable]
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public void SetDirection(Direction dir)
    {
        _direction = dir;
    }

    private void Update()
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
        transform.position += delta * amt;
    }
}
