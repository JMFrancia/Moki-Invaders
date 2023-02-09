using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject _shotPrefab;

    public bool Alive => gameObject.activeInHierarchy;
    public Action OnDestroyed;
    
    public void ResetEnemy()
    {
        gameObject.SetActive(true);
    }

    public void FireShot()
    {
        Instantiate(_shotPrefab, transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!Alive)
            return;
        if (col.gameObject.layer == LayerMask.NameToLayer(Constants.Layers.PLAYER_SHOT))
        {
            Die();
            Destroy(col.gameObject); //Destroy shot
        }
    }
    
    private void Die()
    {
        OnDestroyed?.Invoke();
        gameObject.SetActive(false);
    }
}
