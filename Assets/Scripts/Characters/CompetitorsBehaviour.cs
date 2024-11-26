using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CompetitorsBehaviour : VehicleBehaviour
{
    private float _aiSpeed;
    private Vector3 _direction;

    private Vector3 _knotTargetPosition;

    public bool isNearCorner;
    
    public Transform itemInSight = null;
    
    private void Start()
    {
        _aiSpeed = Random.Range(20, 25);

        GetNewKnotPosition();

        CarMotorAudioGoing.pitch *= 0.5f;
    }

    public override void MoveLogic()
    {
        
        if(Vector3.Distance(transform.position,trackManagerRef.spline.Spline[characterRef.checkPointsReached % trackManagerRef.spline.Spline.Count].Position) < 25)
        {   
            characterRef.ReachedCheckPoint();
            GetNewKnotPosition();
        }
        
        Vector3 direction;

        if (itemInSight != null)
        {
            direction = (itemInSight.position - transform.position).normalized;

            if (Vector3.Distance(transform.position, itemInSight.position) <= 5)
            {
                itemInSight = null;
            }
        }
        else
        {
            itemInSight = null;
            direction = (_knotTargetPosition - transform.position).normalized;
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
        
        GetClosestCurveKnot();
        GetClosestItemBox();
        GetClosestObstacleOnTrack();
    }

    public void GetNewKnotPosition()
    {
        if (characterRef.checkPointsReached >= trackManagerRef.spline.Spline.Count)
        {
            _knotTargetPosition = trackManagerRef.lapCheckPoint.position;
        }
        else
        {
            _knotTargetPosition = new Vector3(trackManagerRef.spline.Spline[characterRef.checkPointsReached].Position.x,trackManagerRef.spline.Spline[characterRef.checkPointsReached].Position.y,trackManagerRef.spline.Spline[characterRef.checkPointsReached].Position.z) + transform.right * Random.Range(-3,3);
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

    private void GetClosestItemBox()
    {
        if (inventoryItem != null) return;

        itemInSight = trackManagerRef.itemBoxPositions
            .Where(itemBox =>
                Vector3.Distance(transform.position, itemBox.position) >= 5 &&
                Vector3.Distance(transform.position, itemBox.position) <= 70 &&
                Vector3.Dot(transform.forward, (itemBox.position - transform.position).normalized) > 0.9)
            .OrderBy(item => Vector3.Distance(transform.position, item.position))
            .FirstOrDefault();
    }
    
    private void GetClosestObstacleOnTrack()
    {
        bool shouldJump = trackManagerRef.obstaclesOnTrackPositions
            .Where(item =>
                Vector3.Distance(transform.position, item.position) <= 10 &&
                Vector3.Dot(transform.forward, (item.position - transform.position).normalized) > 0.9)
            .OrderBy(item => Vector3.Distance(transform.position, item.position))
            .FirstOrDefault();

        if (shouldJump)
        {
            Jump();
        }
    }

    
    private void GetClosestCurveKnot()
    {
        var closestKnot = trackManagerRef.spline.Spline
            .OrderBy(knot => Vector3.Distance(transform.position, knot.Position))
            .FirstOrDefault();

        isNearCorner = Vector3.Distance(transform.position, closestKnot.Position) < 20;
    }

    
}
        
    
