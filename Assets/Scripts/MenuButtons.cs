using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour, ISelectHandler
{
    private enum ButtonAction
    {
        None,
        LoadMainMenu,
        LoadTrackSelection,
        QuitGame,
    }

    [SerializeField] private ButtonAction action;

    private void Start()
    {
        Button button = GetComponent<Button>();
        
        button.onClick.AddListener(HandleButtonClick);
       
    }

    private void HandleButtonClick()
    {
        switch (action)
        {
            case ButtonAction.None:
                break;
            case ButtonAction.LoadMainMenu:
                LoadScene("Main Menu");
                break;
            case ButtonAction.LoadTrackSelection:
                LoadScene("Track Selection");
                break;
            case ButtonAction.QuitGame:
                Application.Quit();
                break;
        }
    }

    private static void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName) && GameManager.Instance != null)
        {
            GameManager.Instance.LoadScene(sceneName);
        }
    }

    public void OnSelect(BaseEventData data)
    {
        gameObject.GetComponent<UAP_BaseElement>().SelectItem();
        GameManager.Instance.StopReadingUI();
    }

}
