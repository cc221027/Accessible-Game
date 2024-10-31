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
    public float ttsSpeechRate = 10f;
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
            
            string content = "";
            TMP_Text tmpText = element.GetComponentInChildren<TMP_Text>();
            
            if (tmpText != null) { content = tmpText.text; }

            if (!string.IsNullOrEmpty(content))
            {
                float waitTime = content.Length * 0.1f / ttsSpeechRate;
                Debug.Log(content.Length);
                yield return new WaitForSecondsRealtime(waitTime);
            }
            else
            {
                yield return new WaitForSecondsRealtime(1.5f);
            }
        }
    }

    public void SetAllVolume(float allV)
    {
        float dbValue = Mathf.Log10(allV / 100f) * 20f;
        masterMixer.SetFloat("masterVolume", dbValue);
        allVolume = allV;
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
        WindowsTTS.SetSpeechVolume((int)hapticsV);  
    }
    
    


    
}
