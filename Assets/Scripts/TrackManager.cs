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

    private GameObject _player;
    
    [SerializeField] private TextMeshProUGUI lapText;
    [SerializeField] private TextMeshProUGUI lapTextHeader;
    [SerializeField] private TextMeshProUGUI countDownTimerText;
    [SerializeField] private TextMeshProUGUI placementText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI playerSpeedText;
    private int _countDownTimer = 5;
    [SerializeField] private List<Transform> spawnPoints;

    public SplineContainer spline;
    public List<int> curveKnots;


    public Transform lapCheckPoint;
    public int laps;
    
    private bool _raceStarted = false;
    private float _raceTimer = 0f;
    public int currentPlayerSpeed;
    public int currentPlayerLap;
    
    private AudioSource _countDownAudio;
    public AudioSource heartbeatSource;


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
        playerSpeedText.text = Mathf.FloorToInt(currentPlayerSpeed) + " m/h";
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


        placementText.text = _player.GetComponent<CharacterData>().placement + ".";
        
        if (_raceStarted)
        {
            _raceTimer += Time.deltaTime;
            int minutes = Mathf.FloorToInt(_raceTimer / 60F);
            int seconds = Mathf.FloorToInt(_raceTimer % 60F);
            int milliseconds = Mathf.FloorToInt((_raceTimer * 1000F) % 1000F);
            string timeFormatted = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
            timeText.text = timeFormatted;
        }
        
        GetClosestCurveKnot();
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
        _player = Instantiate(GameManager.Instance.allCharacters[GameManager.Instance.selectedCharacterIndex], spawnPosition, spawnRotation);

        _player.tag = "Player";
        
       
        if (Camera.main != null)
        {
            Camera.main.transform.SetParent(_player.transform);

            Camera.main.transform.localPosition = new Vector3(0, 5, -10); // Example offset
            Camera.main.transform.localRotation = Quaternion.identity;
        }
       
        CompetitorsBehaviour competitorsBehaviour = _player.GetComponent<CompetitorsBehaviour>();
        Destroy(competitorsBehaviour);

        EnemyFOV enemyFOV = _player.GetComponent<EnemyFOV>();
        Destroy(enemyFOV);

        Transform fieldOfViewTransform = _player.transform.Find("FieldOfView");
        Destroy(fieldOfViewTransform.gameObject);
        
        StartCoroutine(DelayStartMovement(5f, _player));

    }
    
    public void SpawnOpponents(int index, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        GameObject opponent = Instantiate(GameManager.Instance.allCharacters[index], spawnPosition, spawnRotation);

        opponent.tag = "Opponent";
        
        PlayerInput playerInput = opponent.GetComponent<PlayerInput>();
        Destroy(playerInput);

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
    

    private void GetClosestCurveKnot()
    {
        float closestDistance = Mathf.Infinity;
        int closestIndex = -1;

        for (int i = 0; i < curveKnots.Count; i++)
        {
            float distance = Vector3.Distance(_player.transform.position, spline.Spline[i].Position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        //TriggerCurveFeedback(spline.Spline[closestIndex]);    

       
    }

    private void TriggerCurveFeedback(BezierKnot closestCurveKnot)
    {
        Vector3 closestCurvePosition = new Vector3(closestCurveKnot.Position.x, closestCurveKnot.Position.y, closestCurveKnot.Position.z);
        float distanceToCurve = Vector3.Distance(_player.transform.position, closestCurvePosition);
        float proximityFactor = Mathf.InverseLerp(100f, 10f, distanceToCurve); 
        float speedFactor = _player.GetComponent<Rigidbody>().velocity.magnitude / 50;
        float heartbeatRate = Mathf.Lerp(0.5f, 2f, proximityFactor * speedFactor);
        
        heartbeatSource.pitch = heartbeatRate;
        
        if (!heartbeatSource.isPlaying)
        {
            heartbeatSource.Play();
        }
    }
    
   
}
