using UnityEngine;

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
