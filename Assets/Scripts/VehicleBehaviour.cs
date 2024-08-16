using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector3 = UnityEngine.Vector3;

public class VehicleBehaviour : MonoBehaviour
{
    public Rigidbody _rb;
    // [SerializeField] private float speed;
    // private float _steerInputValue;
    // private float _accelerationInputValue;
    // private float _decelerateValue;
    // private float _driftValue;
    public bool _isJumping;
    public bool _isGrounded;
    public GameManager _gameManagerRef;

    public void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _gameManagerRef = GameManager.Instance;
        
    }
    
    private void OnJump(InputValue value)
    {
        if (_isJumping) return;
        _rb.AddForce(transform.up*5, ForceMode.Impulse);
        _isJumping = true;
        _isGrounded = false;
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

    private void FixedUpdate()
    {
        MoveLogic();
    }
}
