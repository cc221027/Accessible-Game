using System;
using UnityEngine;

public class CharacterData: MonoBehaviour
{
    public int index;
    public string characterName;
    public int completedLaps = 0;
    
    private TrackManager _trackManager;

    private void Start()
    {
        _trackManager = FindObjectOfType<TrackManager>();
    }
    
    public void CompleteLap()
    {
        completedLaps++;
        Debug.Log("LapsComplete: " + completedLaps);
        if (completedLaps >= _trackManager.Laps)
        {
            _trackManager.EndRace(this);
        }
    }
}