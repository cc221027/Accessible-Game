using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vector2 = System.Numerics.Vector2;

public class RaceSelection : MonoBehaviour, ICancelHandler
{
    private int _characterIndex;
    private int _trackIndex;
    
    private enum Type
    {
        Track,
        Character,
    }

    [SerializeField] private Type type;

    [Header("UI Elements")] 
    private Vector3 _initialPosition;
    [SerializeField] private Transform selectedButtonTargetPosition;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject readyText;
    [SerializeField] private GameObject[] disabledObjects;

    private Button _selectedButton;
    private bool _isHighlighted = false;

    void Start()
    {
        _initialPosition = transform.position;
        switch (type)
        {
            case Type.Track:
                _trackIndex = transform.GetSiblingIndex();
                GetComponent<Button>().onClick.AddListener(() => HandleSelection(Type.Track));
                break;
            case Type.Character:
                _characterIndex = transform.GetSiblingIndex();
                GetComponent<Button>().onClick.AddListener(() => HandleSelection(Type.Character));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void HandleSelection(Type selectionType)
    {
        if (!_isHighlighted)
        {
            _selectedButton = GetComponent<Button>();
            MoveButtonToHighlight();
            
            ShowInfoPanel();
            
            _isHighlighted = true;
        }
        else
        {
            ConfirmSelection(selectionType);
        }
    }

    private void ConfirmSelection(Type selectionType)
    {
        if (selectionType == Type.Track)
        {
            GameManager.Instance.SelectTrack(_trackIndex); 
            
        }else if (selectionType == Type.Character) 
        { 
            GameManager.Instance.SelectCharacter(_characterIndex);
        }
        _isHighlighted = false;
    }

    private void MoveButtonToHighlight()
    {
        transform.position = selectedButtonTargetPosition.position;
    }

    private void ShowInfoPanel()
    {
        infoPanel.SetActive(true);
        readyText.SetActive(true);
        foreach (var element in disabledObjects)
        {
            element.SetActive(false);
        }
    }

    public void OnCancel(BaseEventData eventData)
    {
        if (_isHighlighted)
        {
            infoPanel.SetActive(false);
            readyText.SetActive(false);
            foreach (var element in disabledObjects)
            {
                element.SetActive(true);
            }
            transform.position = _initialPosition;
            _isHighlighted = false;
        }
        else
        {
            if (SceneManager.GetActiveScene().name == "Character Selection")
            {
                GameManager.Instance.ResetSelection();
                SceneManager.LoadScene("Track Selection");
            } else if (SceneManager.GetActiveScene().name == "Track Selection")
            {
                SceneManager.LoadScene("Main Menu");
            }
        }
        
       
    }
}
