using UnityEngine;

/*
 * Destroys object on game over.
 * TODO: Refactor with ability to choose any event from a drop-down, not just game start
 */
public class DestroyOnGameStart : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.StartListening(Constants.Events.GAME_START, OnGameStart);
    }
    
    private void OnDisable()
    {
        EventManager.StopListening(Constants.Events.GAME_START, OnGameStart);
    }

    void OnGameStart()
    {
        if (gameObject.activeInHierarchy)
        {
            ObjectPoolManager.Release(gameObject, true);
        }
    }
}
