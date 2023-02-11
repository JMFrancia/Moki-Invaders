using UnityEngine;

/*
 * Simple component to set animator speed.
 * Makes it easy to set or change speeds on multiple game objects from the inspector.
 */
[RequireComponent(typeof(Animator))]
public class SetAnimationSpeed : MonoBehaviour
{
    [SerializeField] private float _initialSpeed = 1f;

    private Animator _animator;
    
    public void Set(float speed)
    {
        _animator.speed = speed;
    }
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        Set(_initialSpeed);
    }
}
