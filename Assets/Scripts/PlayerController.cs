using System;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
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
    
    private float _rangeToLeftOffroad;
    private float _rangeToRightOffroad;
    
    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _gamepad = Gamepad.current;
    }

    private void Update()
    {
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
        if (other.CompareTag("LeftOffroad"))
        {
            Gamepad.current.SetMotorSpeeds(0.1f, 0);
        }
        if (other.CompareTag("RightOffroad"))
        {
            Gamepad.current.SetMotorSpeeds(0, 0.1f);
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
