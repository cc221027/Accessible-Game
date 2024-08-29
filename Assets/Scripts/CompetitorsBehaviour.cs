using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CompetitorsBehaviour : VehicleBehaviour
{
    public List<Transform> checkpoints = new List<Transform>(); // List of checkpoint transforms
    private float _aiSpeed; // Speed of the AI
    private Vector3 _direction;
    public int _characterCheckpoints;
    
    public Transform itemInSight = null;
    public bool foundItem;


    private readonly Collider[] _results = new Collider[100]; // Adjust size as needed


    private void Start()
    {
        checkpoints = trackManagerRef.checkPoints;
        _aiSpeed = Random.Range(15, 26);
    }

    public override void MoveLogic()
    {
        Vector3 direction;

        // Randomize the checkpoint position
        float randomOffset = Random.Range(-5f, 5f); // Adjust the range as needed

        if (itemInSight != null &&
            Vector3.Dot(transform.forward, (itemInSight.position - transform.position).normalized) < 0)
        {
            direction = (itemInSight.position - transform.position).normalized;
            if (Vector3.Distance(transform.position, itemInSight.position) <= 20)
            {
                itemInSight = null;
            }
        }
        else
        {
            itemInSight = null;
            _characterCheckpoints = characterRef.checkPointsReached;

            Vector3 checkpointTargetPosition;

            if (_characterCheckpoints >= checkpoints.Count)
            {
                checkpointTargetPosition = trackManagerRef.lapCheckPoint.position +
                                           new Vector3(Random.Range(-randomOffset, randomOffset), 0,
                                               Random.Range(-randomOffset, randomOffset));
            }
            else
            {
                checkpointTargetPosition = checkpoints[_characterCheckpoints].position +
                                           new Vector3(Random.Range(-randomOffset, randomOffset), 0,
                                               Random.Range(-randomOffset, randomOffset));
            }

            direction = (checkpointTargetPosition - transform.position).normalized;
        }

        if (_rb.velocity.magnitude <= maxSpeed)
        {
            _rb.AddForce(transform.forward * _aiSpeed * characterRef.characterAcceleration, ForceMode.Acceleration);
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5);

        if (ShouldUseItem())
        {
            UseItem();
        }
    }

    public override void UseItem()
    {
        if (inventoryItem)
        {
            inventoryItem.GetComponent<ItemBase>().UseItem(gameObject);
            inventoryItem = null;
        }
    }

    private bool ShouldUseItem()
    {
        if (maxSpeed == 25)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void SlowDown()
    {
        Vector3 opposingForce = -transform.forward * _aiSpeed; // Apply an opposing force
        _rb.AddForce(opposingForce, ForceMode.Acceleration);
    }
}
        
    
