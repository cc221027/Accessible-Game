using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MenuButtons : MonoBehaviour
{
    private enum ButtonAction
    {
        LoadMainMenu,
        LoadCharacterSelection,
        LoadRaceTrack1,
        QuitGame,
        ReturnToTrackSelection,
        ReturnToMainMenu
    }

    [SerializeField] private ButtonAction action;

    private void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(HandleButtonClick);
        }
    }

    private void HandleButtonClick()
    {
        switch (action)
        {
            case ButtonAction.LoadMainMenu:
                LoadScene("Main Menu");
                break;
            case ButtonAction.LoadCharacterSelection:
                LoadScene("Track Selection");
                break;
            case ButtonAction.LoadRaceTrack1:
                LoadScene("Main Menu");
                break;
            case ButtonAction.QuitGame:
                Application.Quit();
                break;
            case ButtonAction.ReturnToTrackSelection:
                GameManager.Instance.ResetSelection();
                LoadScene("Track Selection");
                break;
            case ButtonAction.ReturnToMainMenu:
                GameManager.Instance.ResetSelection();
                LoadScene("Main Menu");
                break;
            
        }
    }

    private static void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName) && GameManager.Instance != null)
        {
            GameManager.Instance.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene name is empty or ScenesManager instance not found!");
        }
    }
}

