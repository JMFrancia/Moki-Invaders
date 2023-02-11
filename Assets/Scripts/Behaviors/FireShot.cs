using UnityEngine;

/*
 * Allows game object to fire a shot
 */
public class FireShot : MonoBehaviour
{
    [SerializeField] private GameObject _shotPrefab;
        
    public void Fire()
    {
        GameObject shot = ObjectPoolManager.Get(_shotPrefab, true);
        shot.transform.position = transform.position;
    }
}
