using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * Destroys game object when it comes into contact another object on given layer(s).
 * Set MutualDestruction to true if the other object should be destroyed as well.
 */
[DisallowMultipleComponent]
public class DieOnContact : MonoBehaviour
{
    [Tooltip("Check trigger-based collisions")]
    [SerializeField] private bool _checkTriggers = true;
    [Tooltip("Check regular collisions")]
    [SerializeField] private bool _checkCollisions = true;
    [Tooltip("Type of death: Destroy, Deactivate, or None (useful when allowing callback to handle death)")]
    [SerializeField] private DeathOptions _deathBehavior = DeathOptions.Destroy;
    [Tooltip("Add a contact type for each layer you want to kill this game object")]
    [SerializeField] private ContactType[] _contacts;

    public Action OnDeath;
    
    private int _contactLayerMask;
    private int _mdLayerMask;

    [Serializable]
    private struct ContactType
    {
        public Constants.Layers.LayerEnum Layer;
        [Tooltip("Will destroy colliding game object as well if true")]
        public bool MutualDestruction;
    }

    [Serializable]
    private enum DeathOptions
    {
        Destroy,
        Deactivate,
        None
    }

    private void Awake()
    {
        Initialize();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!_checkTriggers)
            return;
        
        CheckContact(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!_checkCollisions)
            return;
        
        CheckContact(col.gameObject);
    }
    
    private void Initialize()
    {
        List<string> allLayers = new List<string>();
        List<string> mdLayers = new List<string>();
        for (int n = 0; n < _contacts.Length; n++)
        {
            string layerName = Enum.GetName(typeof(Constants.Layers.LayerEnum), _contacts[n].Layer);
            allLayers.Add(layerName);
            if (_contacts[n].MutualDestruction)
            {
                mdLayers.Add(layerName);
            }
        }

        _contactLayerMask = LayerMask.GetMask(allLayers.ToArray());
        _mdLayerMask = LayerMask.GetMask(mdLayers.ToArray());
    }

    void CheckContact(GameObject other)
    {
        if (!other.activeInHierarchy)
            return;
        
        int contactLayer = other.gameObject.layer;
        if (MaskContainsLayer(_contactLayerMask, contactLayer))
        {
            if (MaskContainsLayer(_mdLayerMask, contactLayer))
            {
                ObjectPoolManager.Release(other.gameObject);
            }
            Die();
        }
    }

    //TODO: Move to a utilities class? 
    private bool MaskContainsLayer(LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }

    void Die()
    {
        if (!gameObject.activeInHierarchy)
            return;
        
        OnDeath?.Invoke();
        switch (_deathBehavior)
        {
            case DeathOptions.Deactivate:
                gameObject.SetActive(false);
                break;
            case DeathOptions.Destroy:
                ObjectPoolManager.Release(gameObject, true);
                break;
            case DeathOptions.None:
                break;
            default:
                break;
        }
    }
}
