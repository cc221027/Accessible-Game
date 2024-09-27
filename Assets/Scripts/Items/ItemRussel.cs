using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRussel : ItemBase
{
    private void Awake()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 2)
        {
            PickupAudioSource = audioSources[0];
            UseItemAudio = audioSources[1];
        }  
    }

    private void Start()
    {
        itemName = "Hourglass";
    }
    
    public override void UseItem(GameObject player)
    {
        StartCoroutine(TimeStop(player));
    }

    private IEnumerator TimeStop(GameObject player)
    {
        CharacterData[] allCharacters = FindObjectsOfType<CharacterData>();
        foreach (CharacterData character in allCharacters)
        {
            if (character.gameObject != player)
            {
                character.characterAcceleration *= 0.5f; 
            }
        }

        yield return new WaitForSeconds(5);

        foreach (CharacterData character in allCharacters)
        {
            if (character.gameObject != player) 
            {
                character.characterAcceleration = character.baseCharacterAcceleration; 
            }
        }

        Destroy(gameObject); 
    }
}
