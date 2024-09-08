using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrackManager : MonoBehaviour
{
    public static TrackManager Instance;
    
    [SerializeField] private TextMeshProUGUI lapText;
    [SerializeField] private TextMeshProUGUI lapTextHeader;
    [SerializeField] private TextMeshProUGUI countDownTimerText;
    [SerializeField] private TextMeshProUGUI placementText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI playerSpeedText;
    [SerializeField] private List<Transform> spawnPoints;
    
    [SerializeField] private int checkPointCount;
    [SerializeField] private int laps;
    private int _countDownTimer = 5;
    public int Laps => laps;
    
    public List<Transform> checkPoints = new List<Transform>();
    public Transform lapCheckPoint;
    
    private bool _raceStarted = false;
    private float _raceTimer = 0f;

    public int checkPointsCountRef => checkPointCount;
    void Awake()
    {
        Instance = this;
        StartRace();
        StartCoroutine(CountDownToStart());
    }

    private void Update()
    {
        playerSpeedText.text = Mathf.FloorToInt(GameManager.Instance.currentPlayerSpeed).ToString();
        lapText.text = GameManager.Instance.currentPlayerLap + " / " + laps;
        
        if (_raceStarted)
        {
            _raceTimer += Time.deltaTime;
            int minutes = Mathf.FloorToInt(_raceTimer / 60F);
            int seconds = Mathf.FloorToInt(_raceTimer % 60F);
            int milliseconds = Mathf.FloorToInt((_raceTimer * 1000F) % 1000F);  // Convert the fractional part to milliseconds
            string timeFormatted = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
            timeText.text = timeFormatted;
        }
       
    }

    private void StartRace()
    {
        SpawnCarts();
    }

    void SpawnCarts()
    {
            SpawnSelectedCharacter(spawnPoints[0].position, spawnPoints[0].rotation);

            int opponentIndex = 0;
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                if (i != GameManager.Instance.SelectedCharacterIndex && opponentIndex < spawnPoints.Count - 1)
                {
                    SpawnOpponents(i, spawnPoints[opponentIndex + 1].position, spawnPoints[opponentIndex + 1].rotation);
                    opponentIndex++;
                }
            }
    }
    
    public void SpawnSelectedCharacter(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        GameObject player = Instantiate(GameManager.Instance.allCharacters[GameManager.Instance.SelectedCharacterIndex], spawnPosition, spawnRotation);

        player.tag = "Player";
       
        if (Camera.main != null)
        {
            Camera.main.transform.SetParent(player.transform);

            Camera.main.transform.localPosition = new Vector3(0, 5, -10); // Example offset
            Camera.main.transform.localRotation = Quaternion.identity;
        }
       
        CompetitorsBehaviour competitorsBehaviour = player.GetComponent<CompetitorsBehaviour>();
        Destroy(competitorsBehaviour);

        EnemyFOV enemyFOV = player.GetComponent<EnemyFOV>();
        Destroy(enemyFOV);
       
        StartCoroutine(DelayStartMovement(5f, player));

    }
    
    public void SpawnOpponents(int index, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        GameObject opponent = Instantiate(GameManager.Instance.allCharacters[index], spawnPosition, spawnRotation);
        
        // Disable the PlayerInput component if it exists
        PlayerInput playerInput = opponent.GetComponent<PlayerInput>();
        Destroy(playerInput);

        // Disable the PlayerController script if it exists
        PlayerController playerController = opponent.GetComponent<PlayerController>();
        Destroy(playerController);

        StartCoroutine(DelayStartMovement(5f, opponent));
    }

    private IEnumerator CountDownToStart()
    {
        while (_countDownTimer > 0)
        {
            countDownTimerText.text = _countDownTimer.ToString();
            yield return new WaitForSeconds(1);
            _countDownTimer--;
        }

        countDownTimerText.text = "GO!";
        StartTimer();
        yield return new WaitForSeconds(1);
        countDownTimerText.enabled = false;
    }
    private IEnumerator DelayStartMovement(float delay, GameObject character)
    {
        yield return new WaitForSeconds(delay);
        character.GetComponent<VehicleBehaviour>().EnableMovement();
    }

    private void StartTimer()
    {
        _raceStarted = true;
    }

    public void FinishLap(CharacterData character, List<Transform> checkPoints)
    {
        character.CompleteLap(checkPoints);
        
    }
    
    public void EndRace(CharacterData winner)
    {
        _raceStarted = false;
        // Stop the race and announce the winner
        Debug.Log($"{winner.characterName} has won the race!");
        GameManager.Instance.winner = winner.characterName;
        GameManager.Instance.LoadScene("Result Scene");
        GameManager.Instance.currentPlayerLap = 0;
    }
    
   
}
