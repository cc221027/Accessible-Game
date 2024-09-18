using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour, ISelectHandler
{
    private AudioSource _audioSource;
    private bool _isSceneAudioPlaying;

    private enum ButtonAction
    {
        None,
        LoadMainMenu,
        LoadCharacterSelection,
        QuitGame,
        ReturnToTrackSelection,
        ReturnToMainMenu
    }

    [SerializeField] private ButtonAction action;

    private void Start()
    {
        Button button = GetComponent<Button>();
        _audioSource = GetComponent<AudioSource>();

        if (_audioSource == null)
        {
            Debug.LogError("AudioSource component is missing from this GameObject.");
        }

        if (button != null)
        {
            button.onClick.AddListener(HandleButtonClick);
        }

        // Play the scene audio when the scene is loaded
        PlaySceneAudio();
    }

    private void PlaySceneAudio()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        string audioFilePath = currentSceneName switch
        {
            "Main Menu" => "Audio/Menu/Main Menu",
            "Character Selection" => "Audio/Menu/Select a Character",
            "Track Selection" => "Audio/Menu/Select a Track",
            "Result Scene" => "Audio/Menu/Winner",
            _ => string.Empty
        };

        if (!string.IsNullOrEmpty(audioFilePath))
        {
            AudioClip clip = Resources.Load<AudioClip>(audioFilePath);
            if (clip != null)
            {
                _audioSource.clip = clip;
                _audioSource.Play();
                _isSceneAudioPlaying = true; // Set flag to true
                StartCoroutine(CheckIfSceneAudioIsDone()); // Start checking coroutine
            }
            else
            {
                Debug.LogError($"AudioClip not found at path: {audioFilePath}");
            }
        }
    }

    private IEnumerator CheckIfSceneAudioIsDone()
    {
        while (_audioSource.isPlaying)
        {
            yield return null; // Wait for the next frame
        }
        _isSceneAudioPlaying = false; // Set flag to false when audio is done
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!_isSceneAudioPlaying)
        {
            string audioFilePath = GetAudioFilePathForButton(gameObject.name);
            if (!string.IsNullOrEmpty(audioFilePath))
            {
                AudioClip clip = Resources.Load<AudioClip>(audioFilePath);
                if (clip != null)
                {
                    _audioSource.clip = clip;
                    _audioSource.Play();
                }
            }
        }
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
            case ButtonAction.LoadCharacterSelection:
                LoadScene("Track Selection");
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
    }

    private string GetAudioFilePathForButton(string buttonName)
    {
        return buttonName switch
        {
            "SinglePlayerBtn" => "Audio/Menu/Single Mode",
            "MultiplayerBtn" => "Audio/Menu/Multi Mode",
            "QuitBtn" => "Audio/Menu/Quit",
            "ReturnBtn" => "Audio/Menu/Go Back",
            "MainMenuBtn" => "Audio/Menu/Confirm Button",
            "Character1" => "Audio/Menu/Character 1",
            "Character2" => "Audio/Menu/Character 2",
            "Character3" => "Audio/Menu/Character 3",
            "Character4" => "Audio/Menu/Character 4",
            "Track1" => "Audio/Menu/Track 1",
            "Track2" => "Audio/Menu/Track 2",
            "Track3" => "Audio/Menu/Track 3",
            "Track4" => "Audio/Menu/Track 4",
            "MainMenu" => "Audio/Menu/Main Menu",
            "CharacterSelection" => "Audio/Menu/Select a Character",
            "TrackSelection" => "Audio/Menu/Select a Track",
            "Winner" => "Audio/Menu/Winner",
            _ => string.Empty
        };
    }
}
