using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CheckPointTrigger : MonoBehaviour
{
    public bool checkPointChecked;

    private void OnTriggerEnter(Collider other)
    {
        CharacterData character = other.GetComponent<CharacterData>();
        if (character != null)
        {
            character.ReachedCheckPoint();
            checkPointChecked = true;
        }
    }
}
