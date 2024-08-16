using System;
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
    
    public void CompleteLap()
    {
        if (_readyToFinishLap)
        {
            completedLaps++;
            _readyToFinishLap = false;
        }
        if (completedLaps >= _trackManager.Laps)
        {
            _trackManager.EndRace(this);
        }
    }

    public void ReachedCheckPoint()
    {
        checkPointsReached++;
        if (checkPointsReached >= _trackManager.checkPointsCountRef)
        {
            _readyToFinishLap = true;
            checkPointsReached = 0;
        }
    }
}