using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject page1;
    [SerializeField] private GameObject page2;
    [SerializeField] private GameObject page3;
    [SerializeField] private GameObject page4;
    
    [SerializeField] private GameObject page2ToFocus;
    [SerializeField] private GameObject page3ToFocus;
    [SerializeField] private GameObject page4ToFocus;

    [SerializeField] private List<GameObject> uiToDisable;
    [SerializeField] private GameObject secondUIToDisable;

    [SerializeField] private AudioSource accelerateAudio0;
    [SerializeField] private AudioSource accelerateAudio1;
    [SerializeField] private AudioSource accelerateAudio2;
    [SerializeField] private AudioSource decelerateAudio;
    [SerializeField] private AudioSource jumpAudio;
    [SerializeField] private AudioSource landingAudio;
    [SerializeField] private GameObject jumpButton;

    private GameObject _competitorRef;
    
    private float _accelerationInputValue;

    private bool _testingAcceleration;
    private bool _testingJumping;
    private bool _testingDeceleration;

    

    private int _pageCount = 1;
    
    private void Awake()
    {
        tutorialPanel.SetActive(true);
        foreach (GameObject uiElement in uiToDisable) { uiElement.SetActive(false); }
        Time.timeScale = 0;
    }

    private void Start()
    {
        _competitorRef = FindObjectOfType<CompetitorsBehaviour>().gameObject;
        _competitorRef.GetComponent<CompetitorsBehaviour>().maxSpeed = 26;
    }

    public void StartPageTurnCoroutine()
    {
        StartCoroutine(NextPage());
    }

    private IEnumerator NextPage()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        _pageCount++;
        
        switch (_pageCount)
        {
            case 2:
                page1.SetActive(false);
                page2.SetActive(true);
                EventSystem.current.SetSelectedGameObject(page2ToFocus);
                yield return new WaitForSecondsRealtime(0.2f);
                page2ToFocus.GetComponent<UAP_BaseElement>().SelectItem();
                break;
            case 3:
                page2.SetActive(false);
                page3.SetActive(true);
                EventSystem.current.SetSelectedGameObject(page3ToFocus);
                page3ToFocus.GetComponent<UAP_BaseElement>().SelectItem();
                break;
            case 4:
                page3.SetActive(false);
                page4.SetActive(true);
                _testingAcceleration = false;
                _testingDeceleration = false;
                _testingJumping = false;
                EventSystem.current.SetSelectedGameObject(page4ToFocus);
                page4ToFocus.GetComponent<UAP_BaseElement>().SelectItem();
                break;
        }
        

    }

    private void OnAccelerate(InputValue value)
    {
       
        if (_testingAcceleration)
        {
            _accelerationInputValue = value.Get<float>();
            Debug.Log(_accelerationInputValue);
            if (_accelerationInputValue == 0)
            {
                accelerateAudio1.Stop();
                accelerateAudio2.Stop();
                
                accelerateAudio0.Play();
            }
            else if (_accelerationInputValue >= 0.5 && _accelerationInputValue <= 0.8)
            {
                accelerateAudio0.Stop();
                accelerateAudio2.Stop();
                
                accelerateAudio1.Play();
            }
            else if (_accelerationInputValue >= 0.8)
            {
                accelerateAudio0.Stop();
                accelerateAudio1.Stop();
                
                accelerateAudio2.Play();
            }
        }
    }

    private void OnDecelerate(InputValue value)
    {
        
        if (_testingDeceleration)
        {
            decelerateAudio.Play();
            _testingDeceleration = false;
        }
    }
    
    private void OnJump(InputValue value)
    {
        if (_testingJumping)
        {
            StartCoroutine(TestJumpCoroutine());
            _testingJumping = false;
        }
        
    }
    public void TestAcceleration()
    {
        _testingAcceleration = true;
        _testingDeceleration = false;
        _testingJumping = false;
    }
    
    public void TestDeceleration()
    {
        _testingDeceleration = true;
        _testingAcceleration = false;
        _testingJumping = false;
    }
    
    public void TestJump()
    {
        StartCoroutine(TestJumpDisableButton());
        _testingJumping = true;
        _testingAcceleration = false;
        _testingDeceleration = false;
    }

    private IEnumerator TestJumpCoroutine()
    {
        jumpAudio.Play();
        while (jumpAudio.isPlaying)
        {
            yield return null;
        }
        yield return new WaitForSecondsRealtime( + 0.2f);
        landingAudio.Play();
        jumpButton.SetActive(true);
    }

    private IEnumerator TestJumpDisableButton()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        jumpButton.SetActive(false);
    }

    public void TutorialPanelFinished()
    {
        tutorialPanel.SetActive(false);
        foreach (GameObject uiElement in uiToDisable) { uiElement.SetActive(true); }
        GameManager.Instance.tutorial = false;
        StartCoroutine(TrackManager.Instance.CountDownToStart());
        Time.timeScale = 1;
        
        _testingAcceleration = false;
        _testingDeceleration = false;
        _testingJumping = false;
    }

    public void StartTestR3ButtonCoroutine()
    {
        StopCoroutine(TestR3Button());
        foreach (GameObject element in uiToDisable)
        {
            element.SetActive(false);
        }
        StartCoroutine(TestR3Button());
    }

    private IEnumerator TestR3Button()
    {
        secondUIToDisable.SetActive(false);
        foreach (GameObject element in uiToDisable)
        {
            element.SetActive(true);
            element.GetComponent<UAP_BaseElement>().SelectItem();
            string content = "";
            TMP_Text tmpText = element.GetComponentInChildren<TMP_Text>();
            content = tmpText.text;
            
            float waitTime = content.Length * 0.15f;
            yield return new WaitForSecondsRealtime(waitTime);
            element.SetActive(false);
        }
        secondUIToDisable.SetActive(true);
    }
}
