using System;
using UnityEngine;

public class CharacterData: MonoBehaviour
{
    public int index;
    public string characterName;
    private int _checkPointsReached = 0;
    private int _completedLaps = 0;
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
            _completedLaps++;
            _readyToFinishLap = false;
        }
        if (_completedLaps >= _trackManager.Laps)
        {
            _trackManager.EndRace(this);
        }
    }

    public void ReachedCheckPoint()
    {
        _checkPointsReached++;
        if (_checkPointsReached >= _trackManager.checkPointsCountRef)
        {
            _readyToFinishLap = true;
        }
    }
}