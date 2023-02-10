using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _startScreenParent;
    [SerializeField] private GameObject _gameoverScreenParent;

    void ShowStartScreen()
    {
        _startScreenParent.SetActive(true);
        _gameoverScreenParent.SetActive(false);
    }

    void ShowGameOverScreen()
    {
        _gameoverScreenParent.SetActive(true);
        _startScreenParent.SetActive(false);
    }
}
