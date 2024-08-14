using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceSelection : MonoBehaviour
{
    private int _characterIndex;
    private int _trackIndex;
    
    private enum Type
    {
        Track,
        Character,
    }

    [SerializeField] private Type type;
    
    // Start is called before the first frame update
    void Start()
    {
        switch (type)
        {
            case Type.Track:
                _trackIndex = transform.GetSiblingIndex();
                GetComponent<Button>().onClick.AddListener(SelectTrack);
                break;
            case Type.Character:
                _characterIndex = transform.GetSiblingIndex();
                GetComponent<Button>().onClick.AddListener(SelectCharacter);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    private void SelectTrack()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SelectTrack(_characterIndex);
        }
    }
    private void SelectCharacter()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SelectCharacter(_characterIndex);
        }
    }
}
