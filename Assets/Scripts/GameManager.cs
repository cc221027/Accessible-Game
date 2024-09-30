using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int SelectedCharacterIndex { get; private set; } = -1;
    
    public int SelectedTrackIndex { get; private set; } = -1;

    private bool _readScreenPressed;
    private bool _stopReading = false;

    public string winner;
    public string endTime;
    
    public float sfxVolume = 100f;
    public float musicVolume = 100f;
    public float ttsVolume = 100f;
    public float ttsSpeechRate = 1f;
    public float hapticsVolume = 100f;
    public bool toggleAccessibility = true;
    
    [SerializeField] public List<GameObject> allCharacters = new List<GameObject>();
    [SerializeField] private List<string> trackSceneNames = new List<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        if (sceneName is "Main Menu" or "Result Screen")
        {
            Time.timeScale = 1;
            Gamepad.current.SetMotorSpeeds(0, 0); 
        }
        SceneManager.LoadScene(sceneName);
    }
    
    public void SelectCharacter(int index)
    {
        SelectedCharacterIndex = index;
        SceneManager.LoadScene(trackSceneNames[SelectedTrackIndex]);
    }
    public void SelectTrack(int index)
    {
        SelectedTrackIndex = index;
        SceneManager.LoadScene("Character Selection");

    }
    public void ResetSelection()
    {
        SelectedCharacterIndex = -1;
        SelectedTrackIndex = -1;
    }
    public void OnReadUI()
    {
        _stopReading = false;
        StartCoroutine(ReadUIElements());
    }

    public void StopReadingUI()
    {
        _stopReading = true;  
        StopCoroutine(ReadUIElements());
        
    }

    private IEnumerator ReadUIElements()
    {
        UAP_BaseElement[] uiElements = FindObjectsOfType<UAP_BaseElement>();
        UAP_BaseElement[] sortedElements = uiElements.OrderBy(element => element.m_ManualPositionOrder).ToArray();
    
        foreach (UAP_BaseElement element in sortedElements)
        {
            if (_stopReading) 
                yield break; 
    
            element.SelectItem();
            yield return new WaitForSeconds(1.5f); 
        }
    }
    
    


    
}
