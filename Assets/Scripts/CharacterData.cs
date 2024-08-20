using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterData: MonoBehaviour
{
    public int index;
    public string characterName;
    public int checkPointsReached = 0;
    public int completedLaps = 0;
    private bool _readyToFinishLap;
    private int _maxSpeed;
    
    private TrackManager _trackManager;

    private void Awake()
    {
        _trackManager = FindObjectOfType<TrackManager>();
    }
    
    public void CompleteLap(List<Transform> checkPoints)
    {
        if (_readyToFinishLap)
        {
            completedLaps++;
            _readyToFinishLap = false;
            checkPointsReached = 0;
            if (CompareTag("Player")) {GameManager.Instance.currentPlayerLap++;}

            foreach (var checkPoint in checkPoints)
            {
                checkPoint.GetComponent<CheckPointTrigger>().playersChecked.Remove(name);
            }
        }
        if (completedLaps >= _trackManager.Laps)
        {
            _trackManager.EndRace(this);
        }
    }

    public void ReachedCheckPoint()
    {
        checkPointsReached++;
        Debug.Log("CHECKPOINT REACHED");
        if (checkPointsReached >= _trackManager.checkPointsCountRef)
        {
            _readyToFinishLap = true;
        }
    }
}