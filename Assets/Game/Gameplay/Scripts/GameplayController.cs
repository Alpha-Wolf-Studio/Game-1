using System;

using UnityEngine;

using Newtonsoft.Json;

public class GameplayController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController = null;
    [SerializeField] private GridElementController gridElementController = null;
    [SerializeField] private TextAsset[] jsonLevels = null;

    [Header("------TESTING-------")]
    [SerializeField] private bool applyOverride = false;
    [SerializeField] private TextAsset overrideJsonLevel = null;

    private LevelData currentLevel = null;
    private bool endLevel = false;

    private float timer = 0f;
    private float targetTime = 0f;

    void Start()
    {
        Initialization(
            onSuccess: () =>
            {
                playerController.ToggleInput(true);
            });
    }

    private void Update()
    {
        UpdateTimer();
    }

    private void Initialization(Action onSuccess = null)
    {
        if (applyOverride)
        {
            currentLevel = JsonConvert.DeserializeObject<LevelData>(overrideJsonLevel.text);

            targetTime = currentLevel.levelTime;
        }

        gridElementController.Init(currentLevel, playerController, FinishLevel);
        playerController.Init(gridElementController.UpdatePlayerElementMove, gridElementController.MoveElement, gridElementController.CanMoveElement, IsEndLevel);

        onSuccess?.Invoke();
    }

    private void UpdateTimer()
    {
        if (endLevel)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer > targetTime)
        {
            FinishLevel(true);
        }
    }

    private void FinishLevel(bool win)
    {
        if (endLevel)
        {
            return;
        }

        endLevel = true;
        gridElementController.EndLevel();
        playerController.ToggleInput(false);
    }

    private bool IsEndLevel()
    {
        return endLevel;
    }
}
