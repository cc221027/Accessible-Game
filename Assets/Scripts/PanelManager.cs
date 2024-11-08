using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour, ICancelHandler, ISelectHandler
{
   
    [SerializeField] private GameObject panelRef;
    [SerializeField] private GameObject firstElementToHighlight;
    [SerializeField] private GameObject[] uiToDisable;

    private Slider _slider;
    private Toggle _toggle;

    private bool _paused;
    
    private enum Type
    {
        None,
        SliderSfx,
        SliderMusic,
        SliderTtsVolume,
        SliderTtsSpeechRate,
        SliderHaptics,
        Toggle,
        Pause,
        SliderUI,
        AllVolume,
    }

    [SerializeField] private Type type;

    private void Start()
    {
        
        if (type is Type.SliderSfx or Type.SliderMusic or Type.SliderTtsVolume or Type.SliderHaptics or Type.SliderTtsSpeechRate or Type.SliderUI or Type.AllVolume)
        {
            _slider = gameObject.GetComponent<Slider>();
            
            switch (type)
            {
                case Type.SliderSfx:
                    _slider.value = GameManager.Instance.sfxVolume;
                    break;
                case Type.SliderMusic:
                    _slider.value = GameManager.Instance.musicVolume;
                    break;
                case Type.SliderTtsVolume:
                    _slider.value = GameManager.Instance.ttsVolume;
                    break;
                case Type.SliderTtsSpeechRate:
                    _slider.value = GameManager.Instance.ttsSpeechRate;
                    break;
                case Type.SliderHaptics:
                    _slider.value = GameManager.Instance.hapticsVolume;
                    break;
                case Type.SliderUI:
                    _slider.value = GameManager.Instance.uiVolume;
                    break;
                case Type.AllVolume:
                    _slider.value = GameManager.Instance.allVolume;
                    break;
            }
            _slider.onValueChanged.AddListener(delegate {ValueChanged();});
        }
        if (type == Type.Toggle)
        {
            _toggle = gameObject.GetComponent<Toggle>();
            _toggle.isOn = GameManager.Instance.toggleAccessibility;
            _toggle.onValueChanged.AddListener(delegate { ValueChanged(); });
        }
        
    }

    private void Update()
    {
        if (!_paused && type == Type.Pause)
        {
            EventSystem.current.SetSelectedGameObject(firstElementToHighlight);
            foreach (var element in uiToDisable)
            {
                element.GetComponent<PanelManager>()._paused = true;
            }

            _paused = true;

        }

        if (_paused)
        {
            TrackManager.Instance.paused = true;
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>().enabled = false;
        }
        
        if (Gamepad.current != null)
        {
            if (Mathf.Abs(Gamepad.current.leftStick.ReadValue().y) > 0.1f)
            {
                GameManager.Instance.StopReadingUI();  
            }
        }
        
    }

    public void HandleButtonClick()
    {
        if (uiToDisable.Length > 0) { foreach (var element in uiToDisable) { element.SetActive(false); } }
        panelRef.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstElementToHighlight);
    }

    public void OnCancel(BaseEventData eventData)
    {
        
        AudioClip clip = (AudioClip)Resources.Load("Audio/Menu/Navigation/Negative");
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(clip);
        audioSource.volume = 0.5f;
        
        if (_paused)
        {
            Time.timeScale = 1;
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>().enabled = true;
        }
        else
        {
            foreach (var element in uiToDisable) { element.SetActive(true); }
            EventSystem.current.SetSelectedGameObject(firstElementToHighlight);
        }
        panelRef.SetActive(false);
        if (TrackManager.Instance != null)
        {
            TrackManager.Instance.paused = false;
        }
    }

    private void ValueChanged()
    {
        switch (type)
        {
            case Type.SliderSfx:
                GameManager.Instance.SetSfxVolume(_slider.value); 
                break;
            case Type.SliderMusic:
                GameManager.Instance.SetMusicVolume(_slider.value);  
                break;
            case Type.SliderTtsVolume:
                
                GameManager.Instance.SetTtsVolume(_slider.value); 
                break;
            case Type.SliderTtsSpeechRate:
                GameManager.Instance.SetTtsSpeechRate(_slider.value); 
                break;
            case Type.SliderHaptics:
                GameManager.Instance.SetHapticsVolume(_slider.value); 
                break;
            case Type.Toggle:
                UAP_AccessibilityManager.PauseAccessibility(!_toggle.isOn);
                GameManager.Instance.toggleAccessibility = _toggle.isOn;
                break;
            case Type.SliderUI:
                GameManager.Instance.SetUIVolume(_slider.value); 
                break;
            case Type.AllVolume:
                GameManager.Instance.SetAllVolume(_slider.value); 
                break;
        }
    
        gameObject.GetComponent<UAP_BaseElement>().SelectItem(true);
        GameManager.Instance.StopReadingUI();
    }

    
    public void OnSelect(BaseEventData data)
    {
        AudioClip clip = (AudioClip)Resources.Load("Audio/Menu/Navigation/Interact");
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(clip);
        audioSource.volume = 0.5f;
        
        gameObject.GetComponent<UAP_BaseElement>().SelectItem();
    }

}
