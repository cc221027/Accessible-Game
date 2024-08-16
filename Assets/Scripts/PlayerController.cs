using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector3 = UnityEngine.Vector3;


public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;
    [SerializeField] private float speed;
    private float _steerInputValue;
    private float _accelerationInputValue;
    private float _decelerateValue;
    private float _driftValue;
    private bool _isJumping;
    private bool _isGrounded = true;
    private GameManager _gameManagerRef;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _gameManagerRef = GameManager.Instance;
        
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
        if (_isJumping) return;
        _rb.AddForce(transform.up*5, ForceMode.Impulse);
        _isJumping = true;
        _isGrounded = false;
    }
    
    private void MoveLogic()
    {
        float decelerationForce = _decelerateValue*20;
        float accelerationForce = _accelerationInputValue*20;
        float currentSpeed = _rb.velocity.magnitude;
        bool isDrifting = _driftValue > 0;
        
        // Drifting logic
        float driftFactor = isDrifting ? 0.5f : 1f; // Reduces acceleration during drift
        float rotationAmount = _steerInputValue * 80f * driftFactor * Time.fixedDeltaTime;

        _gameManagerRef.currentPlayerSpeed = currentSpeed;
        
        if (_decelerateValue > 0)
        {
            _rb.AddForce(-_rb.velocity.normalized * Mathf.Min(decelerationForce, currentSpeed), ForceMode.Acceleration);
        }
        else
        {
            _rb.AddForce(transform.forward * (accelerationForce * driftFactor), ForceMode.Acceleration);
        }

        transform.Rotate(0, rotationAmount, 0);
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
            _isJumping = false;
        }
    }

    private void FixedUpdate()
    {
        MoveLogic();
    }
}
