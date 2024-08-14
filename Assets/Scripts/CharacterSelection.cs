using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    public static CharacterSelection Instance { get; private set; }

    [SerializeField] private int selectedCharacterIndex = -1;
    [SerializeField] private GameObject[] characterButtons;
    
    // Start is called before the first frame update
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


    public void SelectCharacter(int index)
    {
        if (index < 0 || index >= characterButtons.Length) return;
        selectedCharacterIndex = index;
        Debug.Log("Character " + index + " selected");
    }

    public void ResetSelection()
    {
        selectedCharacterIndex = -1;
        Debug.Log("Reset Selection");
    }
}
