using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DieOnContact : MonoBehaviour
{
    [SerializeField] private bool _checkTriggers = true;
    [SerializeField] private bool _checkCollisions = true;
    [SerializeField] private DeathOptions _deathBehavior = DeathOptions.Destroy;
    [SerializeField] private ContactType[] _contacts;

    public Action OnDeath;
    
    private int _contactLayerMask;
    private int _mdLayerMask;

    [Serializable]
    private struct ContactType
    {
        public Constants.Layers.LayerEnum Layer;
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

    void Initialize()
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
