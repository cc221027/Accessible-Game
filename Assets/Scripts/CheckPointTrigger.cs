using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CheckPointTrigger : MonoBehaviour
{
    [FormerlySerializedAs("_checkPointChecked")] public bool checkPointChecked;
    private void OnTriggerEnter(Collider other)
    {
        CharacterData character = other.GetComponent<CharacterData>();
        if (character != null && !checkPointChecked)
        {
            character.ReachedCheckPoint();
            checkPointChecked = true;
        }
    }
}
