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
    [HideInInspector] public Rigidbody _rb;
    [HideInInspector] public bool _isJumping;
    [HideInInspector] public bool _isGrounded;
    [HideInInspector] public bool speedReduced = false;
    [HideInInspector] public GameManager _gameManagerRef;
    [HideInInspector] public TrackManager trackManagerRef;
    [HideInInspector] public CharacterData characterRef;
    [HideInInspector] public bool movementEnabled;

    
    [HideInInspector] public int maxSpeed = 50;
    [HideInInspector] public int jumpingPower = 50;
    [HideInInspector] public GameObject inventoryItem;

   

    public void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.drag = 0.1f;
        _gameManagerRef = GameManager.Instance;
        trackManagerRef = TrackManager.Instance;
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item") && inventoryItem == null && Vector3.Distance(gameObject.transform.position, other.transform.position) <= 5)
        {
            other.gameObject.GetComponent<ItemPickupContainer>().GetRandomItem(gameObject);
        }
    }

   

    public virtual void UseItem()
    {
        
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

    public void Jump()
    {
        if (_isJumping) return;
        _rb.AddForce(transform.up * jumpingPower, ForceMode.Impulse);
        _isJumping = true;
        _isGrounded = false;
    }
}
