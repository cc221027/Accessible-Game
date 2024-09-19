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
        LoadTrackSelection,
        QuitGame,
    }

    [SerializeField] private ButtonAction action;

    private void Start()
    {
        Button button = GetComponent<Button>();
        _audioSource = GetComponent<AudioSource>();
        
        button.onClick.AddListener(HandleButtonClick);
        
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
            _ => string.Empty
        };

     
        AudioClip clip = Resources.Load<AudioClip>(audioFilePath);
        if (clip != null)
        {
            _audioSource.clip = clip;
            _audioSource.volume = 0.5f;
            _audioSource.Play();
            _isSceneAudioPlaying = true;
            StartCoroutine(CheckIfSceneAudioIsDone()); 
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
    
    private IEnumerator CheckIfSceneAudioIsDone(string buttonName = null)
    {
        while (_audioSource.isPlaying) 
        {
            yield return null; 
        }
    
        _isSceneAudioPlaying = false;

        if (!string.IsNullOrEmpty(buttonName))
        {
            string audioFilePath = GetAudioFilePathForButton(buttonName);
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

    public void OnSelect(BaseEventData eventData)
    {
        if (_isSceneAudioPlaying)
        {
            StartCoroutine(CheckIfSceneAudioIsDone(gameObject.name));
        }
        else
        {
            string audioFilePath = GetAudioFilePathForButton(gameObject.name);
            if (!string.IsNullOrEmpty(audioFilePath))
            {
                AudioClip clip = Resources.Load<AudioClip>(audioFilePath);
                if (clip != null)
                {
                    if (clip.name == "Confirm Button")
                    {
                        StartCoroutine(WaitForSecondsCoroutine(3, clip));
                      
                    }
                    else
                    {
                        _audioSource.clip = clip;
                        _audioSource.Play();
                    }
              
                }
            }
        }
    }


    private string GetAudioFilePathForButton(string buttonName)
    {
        switch (buttonName)
        {
            case "SinglePlayerBtn":
                return "Audio/Menu/Single Mode";
            case "MultiplayerBtn":
                return "Audio/Menu/Multi Mode";
            case "QuitBtn":
                return "Audio/Menu/Quit";
            case "MainMenuBtn":
                return "Audio/Menu/Confirm Button";
            case "Jimmy":
                return "Audio/Menu/Character 1";
            case "Officer Jenkins":
                return "Audio/Menu/Character 2";
            case "Russel":
                return "Audio/Menu/Character 3";
            case "Stella":
                return "Audio/Menu/Character 4";
            case "Track1":
                return "Audio/Menu/Track 1";
            case "Track2":
                return "Audio/Menu/Track 2";
            case "Track3":
                return "Audio/Menu/Track 3";
            case "Track4":
                return "Audio/Menu/Track 4";
            default:
                return string.Empty;
        }
    }

    private IEnumerator WaitForSecondsCoroutine(int seconds, AudioClip clip)
    {
        yield return new WaitForSeconds(seconds);
        _audioSource.clip = clip;
        _audioSource.Play();
    }

}
