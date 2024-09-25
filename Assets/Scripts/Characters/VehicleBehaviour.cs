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
    
    private AudioSource _driftingAudio;
    public AudioSource countDownAudio;
    private AudioSource _carMotorAudioStill;
    private AudioSource _carMotorAudioStart;
    private AudioSource _carMotorAudioGoing;
    private AudioSource _jumpAudio;
    private AudioSource _gearShiftAudio;
    
    protected AudioSource FirstPlaceAudio;
    protected AudioSource SecondPlaceAudio;
    protected AudioSource ThirdPlaceAudio;
    protected AudioSource FourthPlaceAudio;

    protected AudioSource RoundTwo;
    protected AudioSource FinalLapAudio;
    protected AudioSource CollisionAudio;
    private AudioSource[] _audioSources;

    public void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        trackManagerRef = TrackManager.Instance;
        characterRef = GetComponent<CharacterData>();
        movementEnabled = false;
        
        _audioSources = GetComponents<AudioSource>();
        
        if (_audioSources.Length >= 2)
        {
            countDownAudio = _audioSources[0];
            _carMotorAudioStill = _audioSources[1];
            _carMotorAudioStart = _audioSources[2];
            _carMotorAudioGoing = _audioSources[3];
            _driftingAudio = _audioSources[4];
            
            _offroadWarningAudio = _audioSources[5];
            _teleportBackAudio = _audioSources[6];

            _jumpAudio = _audioSources[7];
            _gearShiftAudio = _audioSources[8];

            FirstPlaceAudio = _audioSources[9];
            SecondPlaceAudio = _audioSources[10];
            ThirdPlaceAudio = _audioSources[11];
            FourthPlaceAudio = _audioSources[12];
            RoundTwo = _audioSources[13];
            FinalLapAudio = _audioSources[14];

            CollisionAudio = _audioSources[15];

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
        } else if (other.gameObject.CompareTag("Obstacle") || other.gameObject.CompareTag("Item Wall") || other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Opponent"))
        {
            CollisionAudio.Play();
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
        
        if (_rb.velocity.magnitude == 0 && !_carMotorAudioStill.isPlaying && !_gearShiftAudio.isPlaying)
        {
            _carMotorAudioGoing.Stop();
            _carMotorAudioStart.Stop();
            _gearShiftAudio.Play();
            _carMotorAudioStill.Play();
            
        }
        else if(_rb.velocity.magnitude !=0 && _rb.velocity.magnitude <= 25 && !_carMotorAudioStart.isPlaying && !_gearShiftAudio.isPlaying)
        {
            _carMotorAudioGoing.Stop();
            _carMotorAudioStill.Stop();
            _gearShiftAudio.Play();
            _carMotorAudioStart.Play();
        }
        else if (_rb.velocity.magnitude > 25 && !_carMotorAudioGoing.isPlaying && !_gearShiftAudio.isPlaying)
        {
            _carMotorAudioStill.Stop();
            _carMotorAudioStart.Stop();
            _gearShiftAudio.Play(); 
            _carMotorAudioGoing.Play();
            
        }
        
        foreach (var audioSource in _audioSources)
        {
            audioSource.volume = GameManager.Instance.sfxVolume/100;
        }
    }

    public void Jump()
    {
        if (_isJumping) return;
        _rb.AddForce(transform.up*_jumpingPower, ForceMode.Impulse);
        _isJumping = true;
        _isGrounded = false;
        
        _jumpAudio.Play();
    }
}
