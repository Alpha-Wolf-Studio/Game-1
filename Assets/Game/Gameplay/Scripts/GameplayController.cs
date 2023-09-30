using System;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController = null;
    [SerializeField] private GridElementController gridElementController = null;

    void Start()
    {
        Initialization(
            onSuccess: () =>
            {
                StartLevel();
            });
    }

    private void Initialization(Action onSuccess = null)
    {
        playerController.Init(gridElementController.UpdatePlayerElementMove, gridElementController.CanMoveElement);
        gridElementController.Init();

        onSuccess?.Invoke();
    }

    private void StartLevel()
    {
        gridElementController.SpawnInitialElements();
    }
}
