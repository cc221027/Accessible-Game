using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterData: MonoBehaviour
{
    public int index;
    
    public string characterName;
    public float baseCharacterAcceleration;
    public float characterAcceleration;
    public float characterWeight;
    
    public int checkPointsReached = 0;
    public int completedLaps = 0;
    private bool _readyToFinishLap;
    
    private TrackManager _trackManager;
    
    private void Awake()
    {
        _trackManager = FindObjectOfType<TrackManager>();
    }
    

    public void CompleteLap()
    {
        if (_readyToFinishLap)
        {
            completedLaps++;
            _readyToFinishLap = false;
            checkPointsReached = 0;
            if (CompareTag("Player")) {TrackManager.Instance.currentPlayerLap++;}
        }
        if (completedLaps >= _trackManager.laps)
        {
            _trackManager.EndRace(this);
        }
    }

    public void ReachedCheckPoint()
    {
        checkPointsReached++;
        if (checkPointsReached >= _trackManager.spline.Spline.Count/3)
        {
            _readyToFinishLap = true;
        }
    }
}