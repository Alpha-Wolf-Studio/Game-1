using System;
using UnityEngine;
using UnityEngine.UI;

public class UiControllerLevelSelector : MonoBehaviour
{
    [SerializeField] private Button backButton = null;
    [SerializeField] private Button[] levelButtons = null;

    public CanvasGroup canvasGroup;
    public event Action onLevelSelectorCloseButtonClicked;

    private void Awake()
    {
        backButton.onClick.AddListener(OnLevelSelectorCloseButtonClicked);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].onClick.AddListener(call: () => PlayLevel(i));
        }
    }

    private void OnDestroy()
    {
        backButton.onClick.RemoveAllListeners();

        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].onClick.RemoveAllListeners();
        }
    }

    private void OnLevelSelectorCloseButtonClicked() => onLevelSelectorCloseButtonClicked?.Invoke();

    private void PlayLevel(int level)
    {
        SceneManager.Instance.LoadSceneAsync("Gameplay");
    }
}
