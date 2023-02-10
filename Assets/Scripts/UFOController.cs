using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOController : MonoBehaviour
{
    // Refactor to Destroy On Shot w/ event
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer(Constants.Layers.PLAYER_SHOT))
        {
            Die();
            ObjectPoolManager.Release(col.gameObject); //Destroy shot
        }
    }

    void Die()
    {
        EventManager.TriggerEvent(Constants.Events.UFO_DESTROYED);
        ObjectPoolManager.Release(gameObject, true);
    }
}
