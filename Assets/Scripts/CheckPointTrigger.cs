using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CheckPointTrigger : MonoBehaviour
{
    public List<String> playersChecked;

    private void OnTriggerEnter(Collider other)
    {
        CharacterData character = other.GetComponent<CharacterData>();
        if (character != null && !playersChecked.Contains(character.name))
        {
            character.ReachedCheckPoint();
            playersChecked.Add(character.name);
        }
    }


}
