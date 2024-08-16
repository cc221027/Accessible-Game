using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapTrigger : MonoBehaviour
{
    [SerializeField] private List<Transform> checkPoints;

    private void Awake()
    {
        GameManager.Instance.checkPoints = checkPoints;
    }

    private void OnTriggerEnter(Collider other)
    {
       
        CharacterData character = other.GetComponent<CharacterData>();
        if (character == null) return;
        TrackManager.Instance.FinishLap(character);
        foreach (var checkPoint in checkPoints)
        {
            checkPoint.GetComponent<CheckPointTrigger>().checkPointChecked = false;
        }
    }

}
