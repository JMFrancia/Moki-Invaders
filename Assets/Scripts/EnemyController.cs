using System;
using UnityEngine;

[RequireComponent(typeof(FireShot), typeof(DieOnContact))]
public class EnemyController : MonoBehaviour
{
    public bool Alive => gameObject.activeInHierarchy;
    public Action OnDestroyed;

    private FireShot _fireShot;

    private void Awake()
    {
        _fireShot = GetComponent<FireShot>();
        GetComponent<DieOnContact>().OnDeath += Die;
    }

    public void ResetEnemy()
    {
        gameObject.SetActive(true);
    }

    public void FireShot()
    {
        _fireShot.Fire();
        EventManager.TriggerEvent(Constants.Events.ENEMY_SHOT_FIRED);
    }
    
    private void Die()
    {
        OnDestroyed?.Invoke();
    }
}
