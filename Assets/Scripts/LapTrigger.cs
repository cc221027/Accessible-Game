using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapTrigger : MonoBehaviour
{
    
    private void Start()
    {
        TrackManager.Instance.lapCheckPoint = transform;    
    }

    private void OnTriggerEnter(Collider other)
    {
        CharacterData character = other.GetComponent<CharacterData>();
        if (character == null) return;
        character.CompleteLap();
    }

}
