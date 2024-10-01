using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CompetitorsBehaviour : VehicleBehaviour
{
    private float _aiSpeed; // Speed of the AI
    private Vector3 _direction;
    public int characterCheckpoints;

    public bool isNearCorner;
    
    public Transform itemInSight = null;
    private void Start()
    {
        _aiSpeed = Random.Range(18, 23);
    }

    public override void MoveLogic()
    {
        characterCheckpoints = characterRef.checkPointsReached;
        
        if(Vector3.Distance(transform.position,trackManagerRef.spline.Spline[characterCheckpoints % trackManagerRef.spline.Spline.Count].Position) < 25)
        {   
            characterRef.ReachedCheckPoint();
        }
        
        Vector3 direction;

        float randomOffset = Random.Range(-10f, 10f);

        if (itemInSight != null)
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
            Vector3 checkpointTargetPosition;

            if (characterCheckpoints >= trackManagerRef.spline.Spline.Count)
            {
                checkpointTargetPosition = trackManagerRef.lapCheckPoint.position + new Vector3(randomOffset, 0, randomOffset);
            }
            else
            {
                checkpointTargetPosition = new Vector3(trackManagerRef.spline.Spline[characterCheckpoints].Position.x, trackManagerRef.spline.Spline[characterCheckpoints].Position.y, trackManagerRef.spline.Spline[characterCheckpoints].Position.z) + new Vector3(Random.Range(-randomOffset, randomOffset), 0, Random.Range(-randomOffset, randomOffset));
            }

            direction = (checkpointTargetPosition - transform.position).normalized;
        }

        if (_rb.velocity.magnitude <= maxSpeed)
        {
            _rb.AddForce(transform.forward * _aiSpeed * characterRef.characterAcceleration, ForceMode.Acceleration);
        }
        if (_rb.velocity.magnitude > 20 && isNearCorner)
        {
            _rb.AddForce(-_rb.velocity.normalized * 50, ForceMode.Acceleration);
        }

        Vector3 flatDirection = new Vector3(direction.x, 0, direction.z);

        if (flatDirection.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatDirection);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5);
        }
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
        if (inventoryItem)
        {
            if (!isNearCorner && inventoryItem.GetComponent<ItemBase>().itemName == "Speedboost")
            {
                return true;    
            }
            if(inventoryItem.GetComponent<ItemBase>().itemName != "Speedboost")
            {
                return true;
            }
        }
        return false;
    }
    
}
        
    
