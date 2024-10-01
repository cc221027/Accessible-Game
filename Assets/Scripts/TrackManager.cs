using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Splines;

public class TrackManager : MonoBehaviour
{
    public static TrackManager Instance;
    
    [SerializeField] private TextMeshProUGUI lapText;
    [SerializeField] private TextMeshProUGUI lapTextHeader;
    [SerializeField] private TextMeshProUGUI countDownTimerText;
    [SerializeField] private TextMeshProUGUI placementText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI playerSpeedText;
    private int _countDownTimer = 5;
    [SerializeField] private List<Transform> spawnPoints;

    public SplineContainer spline;

    public Transform lapCheckPoint;
    public int laps;
    
    private bool _raceStarted = false;
    private float _raceTimer = 0f;
    public int currentPlayerSpeed;
    public int currentPlayerLap;
    
    private AudioSource _countDownAudio;


    void Awake()
    {
        Instance = this;
        _countDownAudio = gameObject.GetComponent<AudioSource>();
        
        SpawnCarts();
        if (!GameManager.Instance.tutorial)
        {
            StartCoroutine(CountDownToStart());
        }
        spline = GameObject.FindGameObjectWithTag("Spline").GetComponent<SplineContainer>();
    }

    private void Update()
    {
        playerSpeedText.text = Mathf.FloorToInt(currentPlayerSpeed).ToString();
        lapText.text = currentPlayerLap + " / " + laps;


        List<CharacterData> racers = FindObjectsOfType<CharacterData>().ToList();

        racers.Sort((r1, r2) => 
        {
            int lapComparison = r2.completedLaps.CompareTo(r1.completedLaps);
            if (lapComparison != 0) return lapComparison;
            
            Vector3 nextCheckpointPosition = spline.Spline[Math.Max(r1.checkPointsReached, r2.checkPointsReached) % spline.Spline.Count].Position;
            
            float r1DistanceToNextCheckpoint = Vector3.Distance(r1.transform.position, nextCheckpointPosition);
            float r2DistanceToNextCheckpoint = Vector3.Distance(r2.transform.position, nextCheckpointPosition);
            
            return r1DistanceToNextCheckpoint.CompareTo(r2DistanceToNextCheckpoint);
        });

        for (int i = 0; i < racers.Count; i++)
        {
            racers[i].placement = i + 1;
        }


        placementText.text = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterData>().placement + ".";
        
        if (_raceStarted)
        {
            _raceTimer += Time.deltaTime;
            int minutes = Mathf.FloorToInt(_raceTimer / 60F);
            int seconds = Mathf.FloorToInt(_raceTimer % 60F);
            int milliseconds = Mathf.FloorToInt((_raceTimer * 1000F) % 1000F);
            string timeFormatted = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
            timeText.text = timeFormatted;
        }
    }

    void SpawnCarts()
    {
        
        SpawnSelectedCharacter(spawnPoints[0].position, spawnPoints[0].rotation);

        int opponentIndex = 0;
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            if (i != GameManager.Instance.selectedCharacterIndex && opponentIndex < spawnPoints.Count - 1)
            {
                SpawnOpponents(i, spawnPoints[opponentIndex + 1].position, spawnPoints[opponentIndex + 1].rotation);
                opponentIndex++;
            }
        }
    }
    
    public void SpawnSelectedCharacter(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        GameObject player = Instantiate(GameManager.Instance.allCharacters[GameManager.Instance.selectedCharacterIndex], spawnPosition, spawnRotation);

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

        Transform fieldOfViewTransform = player.transform.Find("FieldOfView");
        Destroy(fieldOfViewTransform.gameObject);
        
        StartCoroutine(DelayStartMovement(5f, player));

    }
    
    public void SpawnOpponents(int index, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        GameObject opponent = Instantiate(GameManager.Instance.allCharacters[index], spawnPosition, spawnRotation);

        opponent.tag = "Opponent";
        
        // Disable the PlayerInput component if it exists
        PlayerInput playerInput = opponent.GetComponent<PlayerInput>();
        Destroy(playerInput);

        // Disable the PlayerController script if it exists
        PlayerController playerController = opponent.GetComponent<PlayerController>();
        Destroy(playerController);

        StartCoroutine(DelayStartMovement(5f, opponent));
    }

    public IEnumerator CountDownToStart()
    {
        _countDownAudio.Play();
        while (_countDownTimer > 0)
        {
            countDownTimerText.text = _countDownTimer.ToString();
            yield return new WaitForSeconds(1);
            _countDownTimer--;
        }

        countDownTimerText.text = "GO!";
        foreach (VehicleBehaviour character in FindObjectsOfType<VehicleBehaviour>())
        {
            character.GetComponent<VehicleBehaviour>().EnableMovement();
        }

        _raceStarted = true;
        yield return new WaitForSeconds(1);
        countDownTimerText.enabled = false;
    }
    private IEnumerator DelayStartMovement(float delay, GameObject character)
    {
        yield return new WaitForSeconds(delay);
    }
    
    public void EndRace(CharacterData winner)
    {
        _raceStarted = false;
        currentPlayerLap = 0;
        int minutes = Mathf.FloorToInt(_raceTimer / 60F);
        int seconds = Mathf.FloorToInt(_raceTimer % 60F);
        int milliseconds = Mathf.FloorToInt((_raceTimer * 1000F) % 1000F);
        string timeFormatted = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        GameManager.Instance.endTime = timeFormatted;
        GameManager.Instance.winner = winner.characterName;
        GameManager.Instance.LoadScene("Result Scene");
    }
    
   
}
