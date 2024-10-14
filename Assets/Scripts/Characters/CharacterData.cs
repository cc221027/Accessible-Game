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
    public int placement;

    public int progressValue;
    public int checkPointsReached = 0;
    public int completedLaps = 0;
    private bool _readyToFinishLap;
    
    private TrackManager _trackManager;
    private PlayerController _playerController;

    public string status;
    
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
            if (CompareTag("Player"))
            {
                TrackManager.Instance.currentPlayerLap++;
                _playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
                _playerController.checkedSplines.Clear();
            } else if (CompareTag("Opponent"))
            {
                gameObject.GetComponent<CompetitorsBehaviour>().GetNewKnotPosition();
            }
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