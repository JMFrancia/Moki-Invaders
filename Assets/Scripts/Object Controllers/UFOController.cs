using UnityEngine;

[RequireComponent(typeof(DieOnContact))]
public class UFOController : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<DieOnContact>().OnDeath += Die;
    }

    void Die()
    {
        EventManager.TriggerEvent(Constants.Events.UFO_DESTROYED);
    }
}
