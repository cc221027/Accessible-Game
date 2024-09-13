using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

public class VehicleBehaviour : MonoBehaviour
{
    [HideInInspector] public Rigidbody _rb;
    [HideInInspector] public bool _isJumping;
    [HideInInspector] public bool _isGrounded;
    [HideInInspector] public bool speedReduced = false;
    [HideInInspector] public TrackManager trackManagerRef;
    [HideInInspector] public CharacterData characterRef;
    [HideInInspector] public bool movementEnabled;

    
    [HideInInspector] public int maxSpeed = 50;
    private  int _jumpingPower = 15;
    [HideInInspector] public GameObject inventoryItem;

    private AudioSource _offroadWarningAudio;
    private AudioSource _teleportBackAudio;

    public void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        trackManagerRef = TrackManager.Instance;
        characterRef = GetComponent<CharacterData>();
        movementEnabled = false;
        
        AudioSource[] audioSources = GetComponents<AudioSource>(); 
        
        if (audioSources.Length >= 2)
        {
            _offroadWarningAudio = audioSources[0];
            _teleportBackAudio = audioSources[1];
        }
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
                maxSpeed = 50;
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
                maxSpeed = 25;
                StartCoroutine(ReturnToCheckPoint());
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

    private IEnumerator ReturnToCheckPoint()
    {
        //_offroadWarningAudio.Play();
        yield return new WaitForSeconds(5);
        if (speedReduced)
        {
            //Teleport vehicle back to last checkpoint
            //_teleportBackAudio.Play();
        }
    }
   

    public virtual void UseItem()
    {
        
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
        
        if (!_isGrounded && _rb.velocity.y < 20)
        {
            _rb.AddForce(Vector3.down * 20f, ForceMode.Acceleration);
        }
    }

    public void Jump()
    {
        if (_isJumping) return;
        _rb.AddForce(transform.up*_jumpingPower, ForceMode.Impulse);
        _isJumping = true;
        _isGrounded = false;
    }
}
