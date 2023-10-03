using UnityEngine;

public class EndGameUIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup gamePlayPanel;
    [SerializeField] private CanvasGroup endGamePanel;
    [SerializeField] private CanvasGroup winPanel;
    [SerializeField] private CanvasGroup losePanel;
    public void ShowWinScreen()
    {
        TurnOffPanel(gamePlayPanel);
        TurnOnPanel(endGamePanel);
        TurnOnPanel(winPanel);
    }

    public void ShowLoseScreen()
    {
        TurnOffPanel(gamePlayPanel);
        TurnOnPanel(endGamePanel);
        TurnOnPanel(losePanel);
    }
    private void TurnOffPanel(CanvasGroup panel)
    {
        panel.alpha = 0;
        panel.blocksRaycasts = false;
        panel.interactable = false;
    }
    private void TurnOnPanel(CanvasGroup panel)
    {
        panel.alpha = 1;
        panel.blocksRaycasts = true;
        panel.interactable = true;
    }

    public void RetryLevel()
    {
        //No se que chota tengo que cargar aca, asi que regenero la escena :P
        PauseManager.UnPause();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Gameplay");
    }

    public void BackToMenu()
    {
        PauseManager.UnPause();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void NextLevel()
    {
        //No se que chota tengo que cargar aca, asi que regenero la escena :P
        PauseManager.UnPause();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Gameplay");
    }


}
