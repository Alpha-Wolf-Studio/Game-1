using System;

using UnityEngine;

using Newtonsoft.Json;

using TMPro;

public class GameplayController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController = null;
    [SerializeField] private GridElementController gridElementController = null;
    [SerializeField] private TextAsset[] jsonLevels = null;
    [SerializeField] private TMP_Text timerText = null;

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
        }
        else
        {
            int selectedLevelIndex = SceneManager.Instance.selectedLevel;
            currentLevel = JsonConvert.DeserializeObject<LevelData>(jsonLevels[selectedLevelIndex].text);
        }

        timer = currentLevel.levelTime;

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

        timer -= Time.deltaTime;
        if (timer < 0f)
        {
            timer = 0f;
            FinishLevel(true);
        }

        int seconds = (int)timer % 60;
        int minutes = (int)timer / 60;
        timerText.text = minutes + ":" + seconds;
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
