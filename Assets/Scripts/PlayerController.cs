using System;
using System.Diagnostics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;
using Debug = UnityEngine.Debug;

public class PlayerController : VehicleBehaviour
{
    private float _steerInputValue;
    private float _accelerationInputValue;
    private float _decelerateValue;
    private float _driftValue;
    private float _itemUseValue;
    
    private bool _isDrifting;
    private bool _driftEnded;
    private float _driftSteerLock;
    
    private PlayerInput _playerInput;

    private Gamepad _gamepad;
    
    private GameObject _splineGameObject;
    private SplineContainer _spline;
    
    private float _rangeToLeftOffroad;
    private float _rangeToRightOffroad;
    
    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _gamepad = Gamepad.current;
        _splineGameObject = GameObject.FindGameObjectWithTag("Spline");
        _spline = _splineGameObject.GetComponent<SplineContainer>();

    }

    private void Update()
    {
        if (_spline != null)
        {
            // Get the closest point on the spline to the player's position
            Vector3 closestPointToPlayer = ClosestPointOnSpline();

            // Calculate the horizontal direction vector of the spline
            Vector3 splineForward = GetSplineForwardDirection(closestPointToPlayer);
            Vector3 splineForwardXZ = new Vector3(splineForward.x, 0, splineForward.z).normalized;

            // Calculate the horizontal direction vector from the closest point to the player
            Vector3 playerPositionXZ = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 closestPointXZ = new Vector3(closestPointToPlayer.x, 0, closestPointToPlayer.z);
            Vector3 splineToPlayer = (playerPositionXZ - closestPointXZ).normalized;

            // Calculate the cross product to determine the side (left or right)
            float side = Vector3.Cross(splineForwardXZ, splineToPlayer).y;

            Debug.Log(side);

            // Determine the position relative to the spline
            if (side > 0)
            {
                Debug.Log("Player is on the right of the spline point.");
            }
            else if (side < 0)
            {
                Debug.Log("Player is on the left of the spline point.");
            }
            else
            {
                Debug.Log("Player is directly in line with the spline point.");
            }
        }
    }

    private Vector3 ClosestPointOnSpline()
    {
        if (_spline == null) return Vector3.zero;

        // For an Auto Spline, you need to find the closest point on the spline curve.
        // This assumes you have a method to get the closest point, or you can compute it.
        float closestDistance = Mathf.Infinity;
        Vector3 closestPoint = Vector3.zero;

        // Iterate through the spline's points
        for (int i = 0; i < _spline.Spline.Count; i++)
        {
            // Example method to get the spline position; replace as necessary
            Vector3 splinePoint = _spline.Spline[i].Position;

            // Calculate the distance from the player's position to the spline point
            float distance = Vector3.Distance(transform.position, splinePoint);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPoint = splinePoint;
            }
        }

        return closestPoint;
    }

    private Vector3 GetSplineForwardDirection(Vector3 point)
    {
        if (_spline == null || _spline.Spline.Count == 0) 
            return Vector3.forward;

        BezierKnot closestKnot = _spline.Spline[0]; // Start with the first knot
        float closestDistance = Vector3.Distance(point, closestKnot.Position);

        // Find the knot closest to the given point
        foreach (var candidate in _spline.Spline)
        {
            float distance = Vector3.Distance(candidate.Position, point);
            if (distance < closestDistance)  // Find the closest knot
            {
                closestDistance = distance;
                closestKnot = candidate;
            }
        }

        // Convert float3 to Vector3 for normalization
        Vector3 tangentOut = new Vector3(closestKnot.TangentOut.x, closestKnot.TangentOut.y, closestKnot.TangentOut.z);

        // Return the normalized forward direction
        return tangentOut.normalized;
    }





    private void OnSteer(InputValue value)
    {
        _steerInputValue = value.Get<float>();
    }

    private void OnAccelerate(InputValue value)
    {
        _accelerationInputValue = value.Get<float>();
    }

    private void OnDecelerate(InputValue value)
    {
        _decelerateValue = value.Get<float>();
    }

    private void OnDrift(InputValue value)
    {
        _driftValue = value.Get<float>();
    }

    private void OnJump(InputValue value)
    {
        if (movementEnabled)
        {
            Jump();
        }
    }

    private void OnItemUse(InputValue value)
    {
        _itemUseValue = value.Get<float>();
        UseItem();
    }

    public override void MoveLogic()
    {
        float decelerationForce = _decelerateValue * 20;
        float accelerationForce = _accelerationInputValue * 20 * characterRef.characterAcceleration;
        float currentSpeed = _rb.velocity.magnitude;
        _isDrifting = _driftValue > 0;

        _gameManagerRef.currentPlayerSpeed = Mathf.RoundToInt(currentSpeed);

        if (_isDrifting && _isGrounded && currentSpeed >= 10)
        {
            if (_steerInputValue != 0)
            {
                Vector3 targetVelocity = transform.forward * accelerationForce;
                _rb.velocity = Vector3.Lerp(_rb.velocity, targetVelocity * 5, Time.fixedDeltaTime);
            }
            
            
            if (_driftSteerLock == 0)
            {
                _driftSteerLock = _steerInputValue;
            }

            // Apply clamping based on the locked direction
            if (_driftSteerLock < 0)
            {
                _steerInputValue = Mathf.Lerp(_steerInputValue, Mathf.Clamp(_steerInputValue, -1f, -0.3f), 0.7f);
               
            }
            else if (_driftSteerLock > 0)
            {
                _steerInputValue = Mathf.Lerp(_steerInputValue, Mathf.Clamp(_steerInputValue, 0.3f, 1f), 0.7f);
            }
        }
        else
        {
            if (_driftSteerLock != 0)
            {
                _steerInputValue = _playerInput.actions["Steer"].ReadValue<float>();
            }
            
            _driftSteerLock = 0;
            
            if (_decelerateValue > 0 && currentSpeed <= maxSpeed)
            {
                _rb.AddForce(-_rb.velocity.normalized * Mathf.Min(decelerationForce, currentSpeed), ForceMode.Acceleration);
            }
            else if(currentSpeed <= maxSpeed)
            {
                _rb.AddForce(transform.forward * accelerationForce, ForceMode.Acceleration);
            }
        }

        float rotationMultiplier = _isDrifting && _isGrounded ? 1.4f : 0.7f;
        float rotationAmount = _steerInputValue * 80f * rotationMultiplier * Time.fixedDeltaTime;

        transform.Rotate(0, rotationAmount, 0);
    }

    public override void UseItem()
    {
        if (_itemUseValue > 0 && inventoryItem != null)
        {
            inventoryItem.GetComponent<ItemBase>().UseItem(gameObject);
            inventoryItem = null;
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        //if (other.CompareTag("LeftOffroad"))
        {
            //Gamepad.current.SetMotorSpeeds(0.1f, 0);
        }
        //if (other.CompareTag("RightOffroad"))
        {
          // Gamepad.current.SetMotorSpeeds(0, 0.1f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("LeftOffroad") || other.CompareTag("RightOffroad"))
        {
            Gamepad.current.SetMotorSpeeds(0, 0);
        }

    }
}
