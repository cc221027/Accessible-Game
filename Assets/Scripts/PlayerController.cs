using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Haptics;
using UnityEngine.Serialization;
using UnityEngine.Splines;
using Debug = UnityEngine.Debug;

public class PlayerController : VehicleBehaviour
{
    private TrackManager _trackManager;

    private float _steerInputValue;
    private float _accelerationInputValue;
    private float _decelerateValue;
    private float _driftValue;
    private float _itemUseValue;

    private bool _isDrifting;
    private bool _driftEnded;
    private float _driftSteerLock;

    private PlayerInput _playerInput;

    private SplineContainer _spline;
    public List<BezierKnot> checkedSplines;

    private float _rangeToLeftOffroad;
    private float _rangeToRightOffroad;
    
    private int _previousPlacement = -1;

    private bool _playedRoundTwoAudio = false;
    private bool _playedFinalAudio = false;
    private float _originalVolume = 1.0f;

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _spline = GameObject.FindGameObjectWithTag("Spline").GetComponent<SplineContainer>();

        countDownAudio.Play();

        _trackManager = TrackManager.Instance;
    }

    private void Update()
    {
        float playerKnotSide = GetKnotSide();
        
        var gamepad = Gamepad.current;
        
        Debug.Log(playerKnotSide);

        if (gamepad is IDualMotorRumble haptics)
        {
            if (playerKnotSide > 0)
            {
                haptics.SetMotorSpeeds(0, 0.004f); 
            }
            else if (playerKnotSide < 0)
            {
                haptics.SetMotorSpeeds(0.004f, 0);  
            }
            else
            {
                haptics.SetMotorSpeeds(0, 0); 
            }
        }

        if (characterRef.placement != _previousPlacement && movementEnabled)
        {
            _previousPlacement = characterRef.placement;
            PlayPlacementAudio(characterRef.placement);
        }

        if (characterRef.completedLaps == 1 && !_playedRoundTwoAudio)
        {
            _playedRoundTwoAudio = true;
            RoundTwo.Play();
        }
        else if (characterRef.completedLaps == 2 && !_playedFinalAudio)
        {
            _playedFinalAudio = true;
            FinalLapAudio.Play();
        }
    }

    private Vector3 GetClosestKnotPosition()
    {
        return _spline.Spline.OrderBy(p => Vector3.Distance(transform.position, p.Position)).First().Position;
    }

    private Vector3 GetKnotOfInterest(Vector3 closestKnot)
    {
        int closestIndex = _spline.Spline.IndexOf(_spline.Spline.OrderBy(p => Vector3.Distance(transform.position, p.Position)).First());
        BezierKnot nextKnot = _spline.Spline[(closestIndex + 1) % _spline.Spline.Count];

        Vector3 nextKnotPosition = new Vector3(nextKnot.Position.x, nextKnot.Position.y, nextKnot.Position.z);
        
        //return ((nextKnotPosition + closestKnot) / 2) - transform.position;
        return ((nextKnotPosition - closestKnot));
    }

    private float GetKnotSide()
    {
        Vector3 closestKnot = GetClosestKnotPosition();
        Vector3 knotOfInterest = GetKnotOfInterest(closestKnot);
        
        return Vector3.Dot( knotOfInterest.normalized,  transform.right);
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





        foreach (var spline in _spline.Spline)
        {
            if (Vector3.Distance(transform.position, spline.Position) < 20 && !checkedSplines.Contains(spline))
            {
                characterRef.ReachedCheckPoint();
                checkedSplines.Add(spline);
            }
        }

        _trackManager.currentPlayerSpeed = Mathf.RoundToInt(currentSpeed);

        if (_isDrifting && _isGrounded && currentSpeed >= 10 && !speedReduced)
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
                _rb.AddForce(-_rb.velocity.normalized * Mathf.Min(decelerationForce, currentSpeed),
                    ForceMode.Acceleration);
            }
            else if (currentSpeed <= maxSpeed)
            {
                _rb.AddForce(transform.forward * accelerationForce, ForceMode.Acceleration);
            }
        }

        float rotationMultiplier = _isDrifting && _isGrounded && !speedReduced ? 1.4f : 0.7f;
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

    private void PlayPlacementAudio(int placement)
    {

        switch (placement)
        {
            case 1:
                SecondPlaceAudio.Stop();
                ThirdPlaceAudio.Stop();
                FourthPlaceAudio.Stop();
                FirstPlaceAudio.Play();
                break;
            case 2:
                FirstPlaceAudio.Stop();
                ThirdPlaceAudio.Stop();
                FourthPlaceAudio.Stop();
                SecondPlaceAudio.Play();
                break;
            case 3:
                FirstPlaceAudio.Stop();
                SecondPlaceAudio.Stop();
                FourthPlaceAudio.Stop();
                ThirdPlaceAudio.Play();
                break;
            case 4:
                FirstPlaceAudio.Stop();
                SecondPlaceAudio.Stop();
                ThirdPlaceAudio.Stop();
                FourthPlaceAudio.Play();
                break;
            default:
                return;
        }
        
    }
}
