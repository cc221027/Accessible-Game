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
            string objectName = gameObject.name; 
            int checkpointNumber;
            
            string numberPart = objectName.Replace("Checkpoint (", "").Replace(")", "");

            if (int.TryParse(numberPart, out checkpointNumber))
            {
                Debug.Log("Character checkpoint reached: " + character.checkPointsReached);
                Debug.Log("Checkpoint number: " + checkpointNumber);
            
                if (character.checkPointsReached == checkpointNumber)
                {
                    character.ReachedCheckPoint();
                    playersChecked.Add(character.name);
                }
            }   
        }
    }


}
