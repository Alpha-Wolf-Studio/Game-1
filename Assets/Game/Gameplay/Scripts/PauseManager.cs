using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup pausePanel = null;
    [SerializeField] private CanvasGroup optionsPanel = null;

    [SerializeField] private float panelShowTime = 1;
    [SerializeField] private float panelHideTime = 1;

    public static bool IsPaused { get; private set; } = false;

    private bool runningCorroutine = false;

    private void Start()
    {
        ResetPanel(pausePanel);
        ResetPanel(optionsPanel);
    }

    private void Update()
    {
        if (!IsPaused)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                if (!runningCorroutine)
                    StartCoroutine(ShowPanel(pausePanel, panelShowTime));
                Pause();
            }

        }
    }

    private IEnumerator ShowPanel(CanvasGroup panel, float showTime = 0)
    {
        if (showTime != 0)
        {
            float t = 0;
            runningCorroutine = true;
            while (t < showTime)
            {
                t += Time.unscaledDeltaTime;
                panel.alpha = t / showTime;
                yield return null;
            }
        }

        panel.alpha = 1;
        panel.blocksRaycasts = true;
        panel.interactable = true;
        runningCorroutine = false;
    }

    private IEnumerator HidePanel(CanvasGroup panel, float hideTime = 0)
    {
        if (hideTime != 0)
        {
            float t = hideTime;
            runningCorroutine = true;
            while (t > 0)
            {
                t -= Time.unscaledDeltaTime;
                //panel.alpha = hideTime / Time.unscaledDeltaTime * t;
                yield return null;
            }
        }

        ResetPanel(panel);
        runningCorroutine = false;
        if (panel == pausePanel)
        {
            UnPause();
        }
    }

    private void ResetPanel(CanvasGroup panel)
    {
        panel.alpha = 0;
        panel.blocksRaycasts = false;
        panel.interactable = false;
    }

    public void ReturnToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void ResumeGame()
    {
        if (!runningCorroutine)
            StartCoroutine(HidePanel(pausePanel, panelHideTime));
    }
    public void OpenOptions()
    {
        if (!runningCorroutine)
            StartCoroutine(ShowPanel(optionsPanel, 0));
    }

    public void CloseOptions()
    {
        if (!runningCorroutine)
            StartCoroutine(HidePanel(optionsPanel, 0));
    }

    public static void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0;
    }

    public static void UnPause()
    {
        IsPaused = false;
        Time.timeScale = 1;
    }
}