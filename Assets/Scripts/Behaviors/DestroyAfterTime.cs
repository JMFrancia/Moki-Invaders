using System.Collections;
using UnityEngine;

/*
 * Destroys Game Object after given period of time
 */
public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float _seconds = 5f;

    private void OnEnable()
    {
        StartCoroutine(DestroyAfterSeconds(_seconds));
    }

    private IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ObjectPoolManager.Release(gameObject, true);
    }
}
