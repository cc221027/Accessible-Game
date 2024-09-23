using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public string winner;
    public string endTime;
    
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

    private void Update()
    {
    }

    public void LoadScene(string sceneName)
    {
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
    
    private void OnReadUI(InputValue value)
    {
        StartCoroutine(ReadUIElements());
    }

    private IEnumerator ReadUIElements()
    {
        UAP_BaseElement[] uiElements = FindObjectsOfType<UAP_BaseElement>();

        UAP_BaseElement[] sortedElements = uiElements.OrderBy(element => element.m_ManualPositionOrder).ToArray();

        foreach (UAP_BaseElement element in sortedElements)
        {
            element.SelectItem();
            
            yield return new WaitForSeconds(1.5f);
        }
    }


    
}
