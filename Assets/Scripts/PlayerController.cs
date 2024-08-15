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

    private void MoveLogic()
    {
        // Calculate the desired force direction in local space
        Vector3 movement = transform.forward * (_accelerationInputValue * 20);

        _rb.AddForce(movement, ForceMode.Acceleration);
        Debug.Log(_rb.velocity.magnitude);

        // Rotate the object
        float rotationAmount = _steerInputValue * 100 * Time.fixedDeltaTime;
        transform.Rotate(0, rotationAmount, 0);
        
    }

    private void FixedUpdate()
    {
        MoveLogic();
    }
}
