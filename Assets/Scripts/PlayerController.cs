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
    private bool _isJumping;
    private bool _isGrounded = true;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
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
        
        if (_decelerateValue > 0)
        {
            if (Vector3.Dot(transform.forward, _rb.velocity) > 0)
            {
                _rb.AddForce(-_rb.velocity.normalized * Mathf.Min(decelerationForce, currentSpeed), ForceMode.Acceleration);
            }
            else
            {
                _rb.AddForce(-transform.forward * decelerationForce, ForceMode.Acceleration);
            }
        }
        else
        {
            _rb.AddForce(transform.forward * accelerationForce, ForceMode.Acceleration);
        }
        
        float rotationAmount = _steerInputValue * 80f  * Time.fixedDeltaTime;
        //* Mathf.Lerp(1f, 0.6f, Mathf.Clamp01(currentSpeed / 30f)))
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
