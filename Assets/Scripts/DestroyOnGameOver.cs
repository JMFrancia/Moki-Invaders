using UnityEngine;

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
