using System;
using UnityEngine;

public class CharacterData: MonoBehaviour
{
    public int index;
    public string characterName;
    private int _completedLaps = 0;
    
    private TrackManager _trackManager;

    private void Awake()
    {
        _trackManager = FindObjectOfType<TrackManager>();
    }
    
    public void CompleteLap()
    {
        _completedLaps++;
        if (_completedLaps >= _trackManager.Laps)
        {
            _trackManager.EndRace(this);
        }
    }
}