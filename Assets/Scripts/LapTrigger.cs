using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapTrigger : MonoBehaviour
{
    [SerializeField] private List<Transform> checkPoints;
    
    private void Start()
    {
        TrackManager.Instance.checkPoints = checkPoints;
        TrackManager.Instance.lapCheckPoint = transform;    
    }

    private void OnTriggerEnter(Collider other)
    {
        CharacterData character = other.GetComponent<CharacterData>();
        if (character == null) return;
        TrackManager.Instance.FinishLap(character, checkPoints);
    }

}
