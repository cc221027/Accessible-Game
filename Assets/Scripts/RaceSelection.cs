using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Vector2 = System.Numerics.Vector2;

public class RaceSelection : MonoBehaviour, ICancelHandler, ISelectHandler
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
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject[] disabledObjects;
    
    void Start()
    {
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
        ConfirmSelection(selectionType);
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
    }
    
    private void ShowInfoPanel()
    {
        infoPanel.SetActive(true);
        foreach (var element in disabledObjects)
        {
            element.SetActive(false);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        ShowInfoPanel();
    }
    public void OnCancel(BaseEventData eventData)
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
