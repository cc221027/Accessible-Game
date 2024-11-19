using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    
    
    private enum ButtonAction
    {
        None,
        LoadTutorial,
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

    private void Update()
    {
        if (Gamepad.current != null)
        {
            if (Mathf.Abs(Gamepad.current.leftStick.ReadValue().y) > 0.1f)
            {
                GameManager.Instance.StopReadingUI(); 
            }
        }
    }

    private void HandleButtonClick()
    {
        AudioClip clip = null;  // Declare the AudioClip once
        AudioSource audioSource = GetComponent<AudioSource>();

        switch (gameObject.name)
        {
            case "Officer Jenkins":
                if (!GameManager.Instance.beatenTrack1)
                {
                    clip = (AudioClip)Resources.Load("Audio/Menu/Navigation/Bounds Reached");
                }
                else
                {
                    clip = (AudioClip)Resources.Load("Audio/Menu/Navigation/Focus Enter");
                }
                break;
            case "Russel":
                if (!GameManager.Instance.beatenTrack2)
                {
                    clip = (AudioClip)Resources.Load("Audio/Menu/Navigation/Bounds Reached");
                }
                else
                {
                    clip = (AudioClip)Resources.Load("Audio/Menu/Navigation/Focus Enter");
                }
                break;
            case "Stella":
                if (!GameManager.Instance.beatenTrack3)
                {
                    clip = (AudioClip)Resources.Load("Audio/Menu/Navigation/Bounds Reached");
                }
                else
                {
                    clip = (AudioClip)Resources.Load("Audio/Menu/Navigation/Focus Enter");
                }
                break;
            case "Track4":
                if (!GameManager.Instance.beatenTrack1 || !GameManager.Instance.beatenTrack2 || !GameManager.Instance.beatenTrack3)
                {
                    clip = (AudioClip)Resources.Load("Audio/Menu/Navigation/Bounds Reached");
                }
                else
                {
                    clip = (AudioClip)Resources.Load("Audio/Menu/Navigation/Focus Enter");
                }
                break;
            default:
                clip = (AudioClip)Resources.Load("Audio/Menu/Navigation/Focus Enter");
                break;
        }

        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
            audioSource.volume = 0.5f;
        }
        
        switch (action)
        {
            case ButtonAction.None:
                break;
            case ButtonAction.LoadTutorial:
                GameManager.Instance.selectedCharacterIndex = 0;
                GameManager.Instance.playerCharacter = "Jimmy";
                GameManager.Instance.tutorial = true;
                LoadScene("Tutorial");
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

   
}
