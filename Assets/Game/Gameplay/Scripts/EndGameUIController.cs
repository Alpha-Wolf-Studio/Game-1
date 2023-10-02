using UnityEngine;

public class EndGameUIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup endGamePanel;
    [SerializeField] private CanvasGroup winPanel;
    [SerializeField] private CanvasGroup losePanel;
    public void ShowWinScreen()
    {
        endGamePanel.alpha = 1;
        endGamePanel.blocksRaycasts = true;
        endGamePanel.interactable = true;
        winPanel.alpha = 1;
        winPanel.blocksRaycasts = true;
        winPanel.interactable = true;
    }

    public void ShowLoseScreen()
    {
        endGamePanel.alpha = 1;
        endGamePanel.blocksRaycasts = true;
        endGamePanel.interactable = true;
        losePanel.alpha = 1;
        losePanel.blocksRaycasts = true;
        losePanel.interactable = true;
    }

}
