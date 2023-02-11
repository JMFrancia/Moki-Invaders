using UnityEngine;

/*
 * Generates explosion prefab on location when game object is disabled.
 */
public class ExplodeOnDisable : MonoBehaviour
{
    [Tooltip("If true, changes the sprite color of the explasion to match this gameobject's sprite color")]
    [SerializeField] private bool _setExplosionColorToMatch = true;
    [SerializeField] GameObject _explosionPrefab;

    private SpriteRenderer _renderer;

    //For when you just gotta explode
    public void ForceExplode()
    {
        Explode();
    }

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void OnDisable()
    {
        if(!gameObject.scene.isLoaded) 
            return;
        
        Explode();
    }

    private void Explode()
    {
        GameObject explosion = ObjectPoolManager.Get(_explosionPrefab, true);
        explosion.transform.position = transform.position;
        explosion.transform.parent = null;

        if (_setExplosionColorToMatch && _renderer != null)
        {
            explosion.GetComponent<SpriteRenderer>().color = _renderer.color;
        }
    }
}
