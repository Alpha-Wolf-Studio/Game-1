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
                playerController.ToggleInput(true);
            });
    }

    private void Initialization(Action onSuccess = null)
    {
        playerController.Init(gridElementController.UpdatePlayerElementMove, gridElementController.MoveElement, gridElementController.CanMoveElement);
        gridElementController.Init();

        onSuccess?.Invoke();
    }

    private void StartLevel()
    {
        gridElementController.SpawnInitialElements();
    }
}
