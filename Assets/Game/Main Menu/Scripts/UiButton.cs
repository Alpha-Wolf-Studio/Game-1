using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using TMPro;

public class UiButton : MonoBehaviour
{
    [SerializeField] private Button button = null;
    [SerializeField] private TMP_Text levelTxt = null;

    public void SetLevelText(int level)
    {
        levelTxt.text = level.ToString();
    }

    public void SetOnClickAction(UnityAction action)
    {
        button.onClick.AddListener(action);
    }

    public void RemoveOnClickAction()
    {
        button.onClick.RemoveAllListeners();
    }
}
