using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector3 = UnityEngine.Vector3;

public class VehicleBehaviour : MonoBehaviour
{
    public Rigidbody _rb;
    public bool _isJumping;
    public bool _isGrounded;
    public GameManager _gameManagerRef;
    public bool movementEnabled;

    public void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.drag = 0.1f;
        _gameManagerRef = GameManager.Instance;
        movementEnabled = false;
    }


    public virtual void MoveLogic()
    {
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
            _isJumping = false;
        }
    }

    public void EnableMovement()
    {
        movementEnabled = true;
    }

    private void FixedUpdate()
    {
        if (movementEnabled)
        {
            MoveLogic();

        }
    }
}
