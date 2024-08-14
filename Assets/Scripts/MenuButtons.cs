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
                LoadScene("Character Selection");
                break;
            case ButtonAction.LoadRaceTrack1:
                LoadScene("Main Menu");
                break;
            case ButtonAction.QuitGame:
                Application.Quit();
                break;
            
        }
    }

    private void LoadScene(string sceneName)
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

