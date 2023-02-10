using UnityEngine;

public class FireShot : MonoBehaviour
{
    [SerializeField] private GameObject _shotPrefab;
        
    public void Fire()
    {
        GameObject shot = ObjectPoolManager.Get(_shotPrefab);
        shot.transform.position = transform.position;
    }
}
