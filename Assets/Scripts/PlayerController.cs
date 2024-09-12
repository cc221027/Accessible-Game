using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
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
    
    private GameObject _splineGameObject;
    private SplineContainer _spline;
    
    private float _rangeToLeftOffroad;
    private float _rangeToRightOffroad;

    private AudioSource _countdownAudio;
    private AudioSource _carMotorAudio;
    private AudioSource _driftingAudio;
    
    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _splineGameObject = GameObject.FindGameObjectWithTag("Spline");
        _spline = _splineGameObject.GetComponent<SplineContainer>();

        AudioSource[] audioSources = GetComponents<AudioSource>();

        if (audioSources.Length >= 2)
        {
            _countdownAudio = audioSources[0];
            _carMotorAudio = audioSources[1];
            _driftingAudio = audioSources[3];
        }
        
        _countdownAudio.Play();

    }

    private void Update()
{
    if (_spline == null) return;

    Vector3 closestPoint = GetClosestPointOnSpline();
    
    Vector3 splineForward = GetSplineForwardDirection(closestPoint);
    Vector3 splineRight = new Vector3(splineForward.z, 0, -splineForward.x).normalized;

    Vector3 playerPosXZ = new Vector3(transform.position.x, 0, transform.position.z);
    Vector3 closestPointXZ = new Vector3(closestPoint.x, 0, closestPoint.z);

    Vector3 splineToPlayer = playerPosXZ - closestPointXZ;
    float side = Vector3.Dot(splineRight, splineToPlayer);

    // Calculate the angle to the next knot
    float angleToNextKnot = GetAngleToNextKnot(closestPoint, transform);
    

    // Motor speeds based on side
    if (side > 0 && !speedReduced)
    {
        Gamepad.current.SetMotorSpeeds(0, Mathf.Clamp(side / 30f, 0, 1));
        if (angleToNextKnot >= 10)
        {
            Debug.Log(side/30);
            StartCoroutine(PulseMotor(Gamepad.current, MotorSide.Left));
        }
    }
    else if (side < 0 && !speedReduced)
    {
        Gamepad.current.SetMotorSpeeds(Mathf.Clamp(-side / 30f, 0, 1), 0);
        if (angleToNextKnot >= 10)
        {
            Debug.Log(side/30);
            StartCoroutine(PulseMotor(Gamepad.current, MotorSide.Right));
        }
    }
    else
    {
        Gamepad.current.SetMotorSpeeds(0, 0);
    }
}

private Vector3 GetClosestPointOnSpline()
{
    return _spline.Spline
        .OrderBy(p => Vector3.Distance(transform.position, p.Position))
        .First()
        .Position;
}

private Vector3 GetSplineForwardDirection(Vector3 point)
{
    var closestKnot = _spline.Spline
        .OrderBy(knot => Vector3.Distance(point, knot.Position))
        .First();

    int closestIndex = _spline.Spline.IndexOf(closestKnot);
    BezierKnot nextKnot = (closestIndex + 1 < _spline.Spline.Count) ? _spline.Spline[closestIndex + 1] : _spline.Spline[Mathf.Max(0, closestIndex - 1)];

    return (new Vector3(nextKnot.Position.x, 0, nextKnot.Position.z) - new Vector3(closestKnot.Position.x, 0, closestKnot.Position.z)).normalized;
}

private float GetAngleToNextKnot(Vector3 point, Transform playerTransform)
{
    // Find the closest knot and the next knot
    var closestKnot = _spline.Spline.OrderBy(knot => Vector3.Distance(point, knot.Position)).First();
    int closestIndex = _spline.Spline.IndexOf(closestKnot);
    BezierKnot nextKnot = (closestIndex + 1 < _spline.Spline.Count) ? _spline.Spline[closestIndex + 1] : _spline.Spline[0];

    Vector3 directionToNextKnot = new Vector3(nextKnot.Position.x, 0, nextKnot.Position.z) - new Vector3(playerTransform.position.x, 0, playerTransform.position.z);
    Vector3 playerForwardXZ = new Vector3(playerTransform.forward.x, 0, playerTransform.forward.z);
    float angleToNextKnot = Vector3.Angle(playerForwardXZ, directionToNextKnot.normalized);

    return angleToNextKnot;

}


private IEnumerator PulseMotor(Gamepad gamepad, MotorSide side)
{
    float duration = 0.1f; // Duration for each pulse
    
    switch (side)
    {
        case MotorSide.Left:
            gamepad.SetMotorSpeeds(1, 0);
            break;
        case MotorSide.Right:
            gamepad.SetMotorSpeeds(0, 1);
            break;
    }

    yield return new WaitForSeconds(duration);

    gamepad.SetMotorSpeeds(0, 0);
}

private enum MotorSide
{
    Left,
    Right
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
}
