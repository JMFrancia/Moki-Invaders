using UnityEngine;

/*
 * Controller class for the UFO
 */
[RequireComponent(typeof(DieOnContact))]
public class UFOController : MonoBehaviour
{
    private bool _destroyed;
    
    private void Awake()
    {
        GetComponent<DieOnContact>().OnDeath += Die;
    }

    private void OnEnable()
    {
        _destroyed = false;
    }

    void Die()
    {
        _destroyed = true;
        EventManager.TriggerEvent(Constants.Events.UFO_DESTROYED);
    }

    private void OnDisable()
    {
        if (!_destroyed)
        {
            EventManager.TriggerEvent(Constants.Events.UFO_ESCAPED);
        }
    }
}
