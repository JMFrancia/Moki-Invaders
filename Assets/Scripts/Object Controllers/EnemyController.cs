using System;
using UnityEngine;

/*
 * Controller class for a single enemy
 */
[RequireComponent(typeof(FireShot), typeof(DieOnContact))]
public class EnemyController : MonoBehaviour
{
    public bool Alive => gameObject.activeInHierarchy;
    public Action OnDestroyed;

    private FireShot _fireShot;

    /*
     * Resets enemy (IE brings back to life)
     */
    public void ResetEnemy()
    {
        gameObject.SetActive(true);
    }

    /*
     * Fires a shot from this enemy
     */
    public void FireShot()
    {
        _fireShot.Fire();
        EventManager.TriggerEvent(Constants.Events.ENEMY_SHOT_FIRED);
    }
    
    private void Awake()
    {
        _fireShot = GetComponent<FireShot>();
        GetComponent<DieOnContact>().OnDeath += Die;
    }

    private void Die()
    {
        OnDestroyed?.Invoke();
    }
}
