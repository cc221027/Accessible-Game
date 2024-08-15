using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed;
    private Vector2 _steerInputValue;
    private float _accelerationInputValue;

    private void OnSteer(InputValue value)
    {
        _steerInputValue = value.Get<Vector2>();
    }

    private void OnAccelerate(InputValue value)
    {
        _accelerationInputValue = value.Get<float>();
    }

    private void MoveLogic()
    {
        rb.AddForce(0, 0, _accelerationInputValue * 300);
        
        Vector2 result = new Vector2(_steerInputValue.x, 0 )* (speed * Time.fixedDeltaTime);
        rb.velocity = result;
        
    }

    private void FixedUpdate()
    {
        MoveLogic();
    }
}
