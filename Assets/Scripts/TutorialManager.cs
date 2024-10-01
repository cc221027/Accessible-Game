using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject page1;
    [SerializeField] private GameObject page2;
    [SerializeField] private GameObject page3;
    [SerializeField] private GameObject page4;
    
    [SerializeField] private GameObject button1ToFocus;
    [SerializeField] private GameObject button2ToFocus;
    [SerializeField] private GameObject button3ToFocus;
    [SerializeField] private GameObject button4ToFocus;

    [SerializeField] private List<GameObject> uiToDisable;

    private int _pageCount = 1;
    
    private void Awake()
    {
        tutorialPanel.SetActive(true);
        foreach (GameObject uiElement in uiToDisable) { uiElement.SetActive(false); }
        EventSystem.current.SetSelectedGameObject(button1ToFocus);
        Time.timeScale = 0;
    }

    public void NextPage()
    {
        _pageCount++;

        switch (_pageCount)
        {
            case 2:
                page1.SetActive(false);
                page2.SetActive(true);
                EventSystem.current.SetSelectedGameObject(button2ToFocus);
                break;
            case 3:
                page2.SetActive(false);
                page3.SetActive(true);
                EventSystem.current.SetSelectedGameObject(button3ToFocus);
                break;
            case 4:
                page3.SetActive(false);
                page4.SetActive(true);
                EventSystem.current.SetSelectedGameObject(button4ToFocus);
                break;
        }
    }

    public void TutorialPanelFinished()
    {
        tutorialPanel.SetActive(false);
        foreach (GameObject uiElement in uiToDisable) { uiElement.SetActive(true); }
        GameManager.Instance.tutorial = false;
        StartCoroutine(TrackManager.Instance.CountDownToStart());
        Time.timeScale = 1;
    }
}
