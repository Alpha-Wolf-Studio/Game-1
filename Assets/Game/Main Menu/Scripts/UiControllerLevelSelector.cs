using CustomSceneSwitcher.Examples.Scripts;
using System;

using UnityEngine;
using UnityEngine.UI;

public class UiControllerLevelSelector : MonoBehaviour
{
    [SerializeField] private Button backButton = null;
    [SerializeField] private UiButton[] levelButtons = null;
    [SerializeField] private ChangeSceneUI changeSceneUI = null;

    public CanvasGroup canvasGroup;
    public event Action onLevelSelectorCloseButtonClicked;

    private void Awake()
    {
        backButton.onClick.AddListener(OnLevelSelectorCloseButtonClicked);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].SetOnClickAction(() => PlayLevel(i));
            levelButtons[i].SetLevelText(i + 1);
        }
    }

    private void OnDestroy()
    {
        backButton.onClick.RemoveAllListeners();

        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].RemoveOnClickAction();
        }
    }

    private void OnLevelSelectorCloseButtonClicked() => onLevelSelectorCloseButtonClicked?.Invoke();

    private void PlayLevel(int level)
    {
        SceneManager.Instance.selectedLevel = level;
        changeSceneUI.ChangeScene();
    }
}
