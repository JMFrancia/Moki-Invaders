using UnityEngine;
using System;

/*
 * Necessary for identifying pooled objects
 */
public class PoolableObject : MonoBehaviour
{
    [Serializable] public enum PoolableType {
        PlayerShot,
        EnemyShot,
        EnemyExplosion,
        UFO,
        PlayerExplosion,
        UFOExplosion
    }

    [SerializeField] PoolableType _poolableType;

    public PoolableType Type => _poolableType;
}
