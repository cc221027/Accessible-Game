using System.Collections.Generic;
using UnityEngine;

public class CompetitorsBehaviour : VehicleBehaviour
{
    public List<Transform> checkpoints = new List<Transform>();  // List of checkpoint transforms
    public float checkpointRadius = 5f;  // Radius to consider checkpoint reached
    public float aiSpeed;  // Speed of the AI
    

    private void Start()
    {
        checkpoints = _gameManagerRef.checkPoints;
    }

    private void Update()
    {
        MoveTowardsCheckpoint();
    }

    private void MoveTowardsCheckpoint()
    {
        CharacterData characterData = GetComponent<CharacterData>();
        int characterCheckPoints = characterData.checkPointsReached;
        Debug.Log("Current Checkpoints Reached: " + characterCheckPoints);
    
        if (characterCheckPoints >= checkpoints.Count) return;
    
        Transform targetCheckpoint = checkpoints[characterCheckPoints];
        Vector3 direction = (targetCheckpoint.position - transform.position).normalized;

        _rb.velocity = direction * aiSpeed;  // Move towards the checkpoint
    }

}
