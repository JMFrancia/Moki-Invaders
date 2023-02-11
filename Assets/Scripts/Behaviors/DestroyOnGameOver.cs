using UnityEngine;

/*
 * Destroys object on game over.
 * TODO: Refactor with ability to choose any event from a drop-down, not just game over
 */
public class DestroyOnGameOver : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.StartListening(Constants.Events.GAME_OVER, OnGameOver);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Constants.Events.GAME_OVER, OnGameOver);
    }

    void OnGameOver(bool win)
    {
        if (gameObject.activeInHierarchy)
        {
            ObjectPoolManager.Release(gameObject, true);
        }
    }
}
