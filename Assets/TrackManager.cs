using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    [SerializeField] private int laps;
    [SerializeField] private List<Transform> spawnPoints; // List of spawn points for player and opponents
    
    void Start()
    {
        SpawnCarts();
    }

    private void Update()
    {
    }

    void SpawnCarts()
    {
            GameManager.Instance.SpawnSelectedCharacter(spawnPoints[0].position, spawnPoints[0].rotation);

            int opponentIndex = 0;
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                if (i != GameManager.Instance.SelectedCharacterIndex && opponentIndex < spawnPoints.Count - 1)
                {
                    GameManager.Instance.SpawnOpponents(i, spawnPoints[opponentIndex + 1].position, spawnPoints[opponentIndex + 1].rotation);
                    opponentIndex++;
                }
            }
    }
    
    
}
