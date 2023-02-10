using System;
using UnityEngine;

[RequireComponent(typeof(FireShot))]
public class EnemyController : MonoBehaviour
{
    public bool Alive => gameObject.activeInHierarchy;
    public Action OnDestroyed;

    private FireShot _fireShot;

    private void Awake()
    {
        _fireShot = GetComponent<FireShot>();
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

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!Alive)
            return;
        if (col.gameObject.layer == LayerMask.NameToLayer(Constants.Layers.PLAYER_SHOT) && col.gameObject.activeInHierarchy)
        {
            ObjectPoolManager.Release(col.gameObject); //Destroy shot
            Die();
        }
    }
    
    private void Die()
    {
        OnDestroyed?.Invoke();
        gameObject.SetActive(false);
    }
}
