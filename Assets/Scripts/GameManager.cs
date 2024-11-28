using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool beatenTrack1;
    public bool beatenTrack2;
    public bool beatenTrack3;
    public bool beatenTrack4;
    public bool unlockedJenkins;
    public bool unlockedRussel;
    public bool unlockedStella;
    public bool unlockedT4;
    
    public int selectedCharacterIndex  = -1;
    
    public int SelectedTrackIndex { get; private set; } = -1;

    private bool _readScreenPressed;
    private bool _stopReading;

    public bool tutorial;
    public string winner;
    public string playerCharacter;
    public string endTime;

    public float allVolume = 100f;
    public float uiVolume = 100f;
    public float sfxVolume = 100f;
    public float musicVolume = 100f;
    public float ttsVolume = 100f;
    public float ttsSpeechRate = 1;
    public float hapticsVolume = 100f;
    public bool toggleAccessibility = true;

    [SerializeField] private AudioMixer masterMixer;
    
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

    private void Start()
    { 
        PlayerData data = SaveSystem.LoadPlayer();
     
        beatenTrack1 = data.beatenTrack1;
        beatenTrack2 = data.beatenTrack2;
        beatenTrack3 = data.beatenTrack3;
        beatenTrack4 = data.beatenTrack4;
            
        allVolume = data.allVolume;
        uiVolume = data.uiVolume;
        sfxVolume = data.sfxVolume;
        musicVolume = data.musicVolume;
        ttsVolume = data.ttsVolume;
        ttsSpeechRate = data.ttsSpeechRate;
        hapticsVolume = data.hapticsVolume;
        toggleAccessibility = data.toggleAccessibility;
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
        selectedCharacterIndex = index;
        playerCharacter = allCharacters[index].name;
        SceneManager.LoadScene(trackSceneNames[SelectedTrackIndex]);
    }
    public void SelectTrack(int index)
    {
        SelectedTrackIndex = index;
        SceneManager.LoadScene("Character Selection");

    }
    public void ResetSelection()
    {
        selectedCharacterIndex = -1;
        SelectedTrackIndex = -1;
    }
    public void OnReadUI()
    {
        StopReadingUI();
        _stopReading = false;
        StartCoroutine(ReadUIElements());
    }

    public void StopReadingUI()
    {
        _stopReading = true; 
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
            EventSystem.current.SetSelectedGameObject(element.gameObject);
            
         
            yield return new WaitForSecondsRealtime(2f);
            
            while (UAP_AccessibilityManager.IsSpeaking())
            {
                yield return null;
            }
         
        }
    }

    public void SetAllVolume(float allV)
    {
        float dbValue = Mathf.Log10(allV / 100f) * 20f;
        masterMixer.SetFloat("masterVolume", dbValue);
        allVolume = allV;
        SetSfxVolume(allV);
        SetUIVolume(allV);
        SetMusicVolume(allV);
        SetTtsVolume(allV);
        SetTtsSpeechRate(allV/10);
        SetHapticsVolume(allV);
    }
    public void SetSfxVolume(float sfxV)
    {
        float dbValue = Mathf.Log10(sfxV / 100f) * 20f;
        masterMixer.SetFloat("sfxVolume", dbValue);
        sfxVolume = sfxV;
    }

    public void SetUIVolume(float uiV)
    {
        float dbValue = Mathf.Log10(uiV / 100f) * 20f;
        masterMixer.SetFloat("uiVolume", dbValue);
        uiVolume = uiV;
    }

    public void SetMusicVolume(float musicV)
    {
        float dbValue = Mathf.Log10(musicV / 100f) * 20f;
        masterMixer.SetFloat("musicVolume", dbValue);
        musicVolume = musicV;
    }

    public void SetTtsVolume(float ttsV)
    {
        float dbValue = Mathf.Log10(ttsV / 100f) * 20f;
        masterMixer.SetFloat("ttsVolume", dbValue);
        ttsVolume = ttsV;
    }

    public void SetTtsSpeechRate(float ttsRate)
    {
        ttsSpeechRate = ttsRate;
        WindowsTTS.SetSpeechRate((int)ttsRate);
    }

    public void SetHapticsVolume(float hapticsV)
    {
        hapticsVolume = hapticsV;
    }
    
}
