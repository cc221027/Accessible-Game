using System.Collections.Generic;
using UnityEngine;

public class CompetitorsBehaviour : VehicleBehaviour
{
    public List<Transform> checkpoints = new List<Transform>();  // List of checkpoint transforms
    public float checkpointRadius = 5f;  // Radius to consider checkpoint reached
    public float aiSpeed;  // Speed of the AI
    private Vector3 _direction;
    

    private void Start()
    {
        checkpoints = _gameManagerRef.checkPoints;
    }

    public override void MoveLogic()
    {
        CharacterData characterData = GetComponent<CharacterData>();
        int characterCheckPoints = characterData.checkPointsReached;
    
        if (characterCheckPoints >= checkpoints.Count)
        { _direction = (_gameManagerRef.lapCheckPoint.position - transform.position).normalized;;
        }
        else
        {
            _direction = (checkpoints[characterCheckPoints].position - transform.position).normalized;
        }
    
       

        

        _rb.velocity = _direction * aiSpeed;  // Move towards the checkpoint
    }

}
