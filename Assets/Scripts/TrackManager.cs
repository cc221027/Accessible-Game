using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    public static TrackManager Instance;
    
    [SerializeField] private int laps;
    [SerializeField] private TextMeshProUGUI lapText;
    [SerializeField] private TextMeshProUGUI lapTextHeader;
    [SerializeField] private TextMeshProUGUI playerSpeedText;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private int checkPointCount;    
    public int Laps => laps;

    public int checkPointsCountRef => checkPointCount;
    void Awake()
    {
        Instance = this;
        StartRace();
    }

    private void Update()
    {
        playerSpeedText.text = Mathf.FloorToInt(GameManager.Instance.currentPlayerSpeed).ToString();
        lapText.text = GameManager.Instance.currentPlayerLap + " / " + laps;
    }

    private void StartRace()
    {
        SpawnCarts();
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
            StartCoroutine(DelayStartMovement(5f));
            
    }

    public void FinishLap(CharacterData character, List<Transform> checkPoints)
    {
        character.CompleteLap(checkPoints);
       
    }
  
    
    public void EndRace(CharacterData winner)
    {
        // Stop the race and announce the winner
        Debug.Log($"{winner.characterName} has won the race!");
        GameManager.Instance.winner = winner.characterName;
        GameManager.Instance.LoadScene("Result Scene");
        GameManager.Instance.currentPlayerLap = 0;
        // Implement race-end logic here, such as stopping all cars, displaying the winner, etc.
    }
    
    private IEnumerator DelayStartMovement(float delay)
    {
        // Wait for the specified amount of time
        yield return new WaitForSeconds(delay);
    
        // Notify all characters to start moving
        GameManager.Instance.EnableCharacterMovement();
    }
}
