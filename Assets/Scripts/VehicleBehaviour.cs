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
    public bool speedReduced = false;
    public GameManager _gameManagerRef;
    public CharacterData characterRef;
    public bool movementEnabled;

    
    public int maxSpeed = 50;
    public int jumpingPower = 5;

    public void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.drag = 0.1f;
        _gameManagerRef = GameManager.Instance;
        characterRef = GetComponent<CharacterData>();
        movementEnabled = false;
    }

    public virtual void MoveLogic()
    {
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Road"))
        {
            _isGrounded = true;
            _isJumping = false;
            if (speedReduced)
            {
                characterRef.characterAcceleration = characterRef.baseCharacterAcceleration;
                speedReduced = false;
            }
        }
        else if (other.gameObject.CompareTag("Offroad"))
        {
            _isGrounded = true;
            _isJumping = false;

            if (!speedReduced)
            {
                characterRef.characterAcceleration *= 0.8f;
                speedReduced = true;
            }
        }
        else if (other.gameObject.CompareTag("Item"))
        {
            other.gameObject.GetComponent<ItemPickupContainer>().GetRandomItem(transform.position, transform.rotation);
        }
    }

    public void EnableMovement()
    {
        _rb.mass = characterRef.characterWeight;
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
