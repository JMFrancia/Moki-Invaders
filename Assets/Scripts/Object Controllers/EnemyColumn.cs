using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyColumn : MonoBehaviour
{
    public Action<EnemyColumn> ColumnDestroyed;
    public int EnemyCount => _enemies?.Count - _destroyedEnemies ?? GetComponentsInChildren<EnemyController>().Length;

    private List<EnemyController> _enemies;
    private int _destroyedEnemies;

    private void Awake()
    {
        //Iterating over childcount instead of using recursive GetComponentsInChildren() in order to guarantee child order
        //for finding the bottom enemy of each column
        _enemies = new List<EnemyController>();
        for (int n = 0; n < transform.childCount; n++)
        {
            var enemy = transform.GetChild(n).GetComponentInChildren<EnemyController>();
            _enemies.Add(enemy);
            enemy.OnDestroyed += OnEnemyDestroyed;
        }
    }

    private void OnEnemyDestroyed()
    {
        _destroyedEnemies++;
        EventManager.TriggerEvent(Constants.Events.ENEMY_DESTROYED);
        if (IsColumnEmpty())
        {
            ColumnDestroyed?.Invoke(this);
        }
    }


    public void ResetColumn()
    {
        for (int n = 0; n < _enemies.Count; n++)
        {
            _enemies[n].ResetEnemy();
        }

        _destroyedEnemies = 0;
    }

    public bool IsColumnEmpty()
    {
        return _destroyedEnemies == _enemies.Count;
    }

    public void FireShot()
    {
        GetBottomEnemy()?.FireShot();
    }

    private EnemyController GetBottomEnemy()
    {
        EnemyController result = null;
        for (int n = _enemies.Count - 1; n >= 0; n--)
        {
            if (_enemies[n].Alive)
            {
                result = _enemies[n];
                break;
            }
        }
        return result;
    }
}
