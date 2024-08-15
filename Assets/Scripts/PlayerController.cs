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

    private void MoveLogic()
    {

        float decelerationForce = _decelerateValue*20;
        float accelerationForce = _accelerationInputValue*20;
        Vector3 currentVelocity = _rb.velocity;
        
        // If decelerating, reduce the forward velocity
        if (_decelerateValue > 0 && currentVelocity.magnitude > 0)
        {
            // Calculate the deceleration force in the direction of the velocity
            float decelerationMagnitude = Mathf.Min(decelerationForce, _rb.velocity.magnitude);
            Vector3 decelerationVector = -_rb.velocity.normalized * decelerationMagnitude;
            _rb.AddForce(decelerationVector, ForceMode.Acceleration);
        } 
        else
        {
            _rb.AddForce(transform.forward * (accelerationForce - decelerationForce), ForceMode.Acceleration);
        }
        float currentSpeed = currentVelocity.magnitude;
        float rotationAmount = _steerInputValue * 
                               (100f * Mathf.Lerp(1f, 0.6f, Mathf.Clamp01(currentSpeed / 30f))) * 
                               Time.fixedDeltaTime;
        transform.Rotate(0, rotationAmount, 0);
        
    }

    private void FixedUpdate()
    {
        MoveLogic();
    }
}
