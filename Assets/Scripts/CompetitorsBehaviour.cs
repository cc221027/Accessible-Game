using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Random = UnityEngine.Random;

public class CompetitorsBehaviour : VehicleBehaviour
{
    public List<Transform> checkpoints = new List<Transform>();  // List of checkpoint transforms
    private float _aiSpeed;  // Speed of the AI
    private Vector3 _direction;
    private int _characterCheckpoints;
    
    private Transform _itemInSight = null;
    private bool _foundItem;
    
    private bool _wentPastItem;
    
    private readonly Collider[] _results = new Collider[100]; // Adjust size as needed

    
    private void Start()
    {
        checkpoints = trackManagerRef.checkPoints;
        _aiSpeed = Random.Range(15, 26);
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * 10, Color.red);
        DrawSphere(transform.position, 80);

        int size = Physics.OverlapSphereNonAlloc(transform.position, 80, _results);
        
        for (int i = 0; i < size; i++)
        {
            var singleCollider = _results[i];

            if (singleCollider.CompareTag("Item"))
            {
                if (Vector3.Distance(transform.position, singleCollider.transform.position) <= 80f && !_foundItem)
                {
                    _itemInSight = singleCollider.transform;
                    _foundItem = true;
                    
                }
            }
        }

        if (Physics.Raycast(transform.position, transform.forward, out var hit, 10f))
        {
            if (hit.collider.CompareTag("Item Wall"))
            {
                Jump();
            }
        }
    }
    
    private void DrawSphere(Vector3 center, float radius)
    {
        int segments = 360;
        float angleStep = 360f / segments;
    
        // Draw the circle on the XZ plane
        for (float angle = 0; angle < 360; angle += angleStep)
        {
            float rad = Mathf.Deg2Rad * angle;
            Vector3 start = center + new Vector3(Mathf.Cos(rad) * radius, 0, Mathf.Sin(rad) * radius);
            float nextRad = Mathf.Deg2Rad * (angle + angleStep);
            Vector3 end = center + new Vector3(Mathf.Cos(nextRad) * radius, 0, Mathf.Sin(nextRad) * radius);
            Debug.DrawLine(start, end, Color.green);
        }

        // Draw the circle on the YZ plane
        for (float angle = 0; angle < 360; angle += angleStep)
        {
            float rad = Mathf.Deg2Rad * angle;
            Vector3 start = center + new Vector3(0, Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius);
            float nextRad = Mathf.Deg2Rad * (angle + angleStep);
            Vector3 end = center + new Vector3(0, Mathf.Cos(nextRad) * radius, Mathf.Sin(nextRad) * radius);
            Debug.DrawLine(start, end, Color.green);
        }

        // Draw the circle on the XZ plane (to close the loop)
        Debug.DrawLine(center + new Vector3(Mathf.Cos(0) * radius, 0, Mathf.Sin(0) * radius),
            center + new Vector3(Mathf.Cos(360) * radius, 0, Mathf.Sin(360) * radius),
            Color.green);
    }

    public override void MoveLogic()
{
    Vector3 direction;

    // Randomize the checkpoint position
    float randomOffset = Random.Range(-5f, 5f); // Adjust the range as needed

    if (_itemInSight != null)
    {
        direction = (_itemInSight.position - transform.position).normalized;
        if (Vector3.Distance(transform.position, _itemInSight.position) < 5)
        {
            _itemInSight = null;
        }
    }
    else
    {
        _characterCheckpoints = characterRef.checkPointsReached;

        Vector3 checkpointTargetPosition;
        if (_characterCheckpoints >= checkpoints.Count)
        { 
            checkpointTargetPosition = trackManagerRef.lapCheckPoint.position + new Vector3(Random.Range(-randomOffset, randomOffset), 0, Random.Range(-randomOffset, randomOffset));
            direction = (checkpointTargetPosition - transform.position).normalized;
        }
        else
        {
            checkpointTargetPosition = checkpoints[_characterCheckpoints].position + new Vector3(Random.Range(-randomOffset, randomOffset), 0, Random.Range(-randomOffset, randomOffset));
            direction = (checkpointTargetPosition - transform.position).normalized;
        }
    }
    if (_rb.velocity.magnitude <= maxSpeed)
    {
        _rb.AddForce(transform.forward*_aiSpeed * characterRef.characterAcceleration, ForceMode.Acceleration);
    }
    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 2);

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
        return true;
    }
}
