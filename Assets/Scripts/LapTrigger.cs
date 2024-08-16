using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapTrigger : MonoBehaviour
{
    [SerializeField] private List<GameObject> checkPoints;
    
    private void OnTriggerEnter(Collider other)
    {
        CharacterData character = other.GetComponent<CharacterData>();
        if (character == null) return;
        character.CompleteLap();
        foreach (var checkPoint in checkPoints)
        {
            checkPoint.GetComponent<CheckPointTrigger>().checkPointChecked = false;
        }
    }

}
