using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject _shotPrefab;

    public bool Alive => gameObject.activeInHierarchy;
    public Action<string> OnDestroyed;
    
    public void ResetEnemy()
    {
        gameObject.SetActive(true);
    }

    public void FireShot()
    {
        Instantiate(_shotPrefab, transform.position - new Vector3(0, .3f, 0), Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!Alive)
            return;
        if (col.CompareTag(Constants.Tags.SHOT))
        {
            Die();
            Destroy(col.gameObject); //Destroy shot
        }
    }
    
    private void Die()
    {
        OnDestroyed?.Invoke(gameObject.name);
        gameObject.SetActive(false);
    }
}
