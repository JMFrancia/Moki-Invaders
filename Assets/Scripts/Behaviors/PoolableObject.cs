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
        Explosion,
        UFO
    }

    [SerializeField] PoolableType _poolableType;

    public PoolableType Type => _poolableType;
}
