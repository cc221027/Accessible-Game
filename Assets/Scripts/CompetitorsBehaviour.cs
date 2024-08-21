using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CompetitorsBehaviour : VehicleBehaviour
{
    public List<Transform> checkpoints = new List<Transform>();  // List of checkpoint transforms
    private float _aiSpeed;  // Speed of the AI
    private Vector3 _direction;
    private int _characterCheckpoints;
    

    
    private void Start()
    {
        checkpoints = _gameManagerRef.checkPoints;
        _aiSpeed = Random.Range(20, 31);
    }

    public override void MoveLogic()
    {
        _characterCheckpoints = characterRef.checkPointsReached;
    
        if (_characterCheckpoints >= checkpoints.Count)
        { _direction = (_gameManagerRef.lapCheckPoint.position - transform.position).normalized;;
        }
        else
        {
            _direction = (checkpoints[_characterCheckpoints].position - transform.position).normalized;
        }
        if (_rb.velocity.magnitude <= maxSpeed)
        {
            _rb.AddForce(_direction * _aiSpeed * characterRef.characterAcceleration, ForceMode.Acceleration);
        }
        UseItem();
    }

    public override void UseItem()
    {
        if (inventoryItem != null)
        {
            Debug.Log("COMPETITOR USED ITEM!");
        }
    }

}
