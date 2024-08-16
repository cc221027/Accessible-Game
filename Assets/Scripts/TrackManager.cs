using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    public static TrackManager Instance;
    
    [SerializeField] private int laps;
    [SerializeField] private TextMeshPro lapText;
    [SerializeField] private TextMeshProUGUI playerSpeedText;
    [SerializeField] private List<Transform> spawnPoints;
    private int checkPointCount;    
    public int Laps => laps;

    public int checkPointsCountRef => checkPointCount;
    void Awake()
    {
        Instance = this;
        SpawnCarts();
    }

    private void Update()
    {
        playerSpeedText.text = Mathf.FloorToInt(GameManager.Instance.currentPlayerSpeed).ToString();
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

    public void FinishLap(CharacterData character)
    {
        character.CompleteLap();
    }
  
    
    public void EndRace(CharacterData winner)
    {
        // Stop the race and announce the winner
        Debug.Log($"{winner.characterName} has won the race!");
        GameManager.Instance.winner = winner.characterName;
        GameManager.Instance.LoadScene("Result Scene");
        // Implement race-end logic here, such as stopping all cars, displaying the winner, etc.
    }
    
    
}
