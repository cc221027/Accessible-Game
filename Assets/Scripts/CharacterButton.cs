using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
{
    [SerializeField] private int characterIndex;
    
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => SelectCharacter());
    }

    private void SelectCharacter()
    {
        if (CharacterSelection.Instance != null)
        {
            CharacterSelection.Instance.SelectCharacter(characterIndex);
        }
    }
}
