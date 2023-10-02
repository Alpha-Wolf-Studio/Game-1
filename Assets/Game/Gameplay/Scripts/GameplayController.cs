using System;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController = null;
    [SerializeField] private GridElementController gridElementController = null;

    private bool endLevel = false;

    void Start()
    {
        Initialization(
            onSuccess: () =>
            {
                playerController.ToggleInput(true);
            });
    }

    private void Initialization(Action onSuccess = null)
    {
        playerController.Init(gridElementController.UpdatePlayerElementMove, gridElementController.MoveElement, gridElementController.CanMoveElement);
        gridElementController.Init(FinishLevel, StartLevel);

        onSuccess?.Invoke();
    }

    private void StartLevel()
    {
        gridElementController.StartLevel(null);
    }

    private void FinishLevel(bool win)
    {
        if (endLevel)
        {
            return;
        }

        endLevel = true;
        playerController.ToggleInput(false);
    }
}
