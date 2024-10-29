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
    
    private AudioSource _carMotorAudioStill;
    private AudioSource _carMotorAudioStart;
    private AudioSource _carMotorAudioGoing;
    private AudioSource _jumpAudio;
    private AudioSource _landingAudio;
    protected AudioSource BrakingAudio;
    private AudioSource _gearShiftAudio;
    
    protected AudioSource FirstPlaceAudio;
    protected AudioSource SecondPlaceAudio;
    protected AudioSource ThirdPlaceAudio;
    protected AudioSource FourthPlaceAudio;

    protected AudioSource RoundTwo;
    protected AudioSource FinalLapAudio;
    private AudioSource _collisionAudio;
    protected AudioSource CollisionWarningAudio;
    
    
    protected AudioSource[] AudioSources;

    public void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        trackManagerRef = TrackManager.Instance;
        characterRef = GetComponent<CharacterData>();
        movementEnabled = false;
        
        AudioSources = GetComponents<AudioSource>();
        
        if (AudioSources.Length >= 2)
        {
            _carMotorAudioStill = AudioSources[0];
            _carMotorAudioStart = AudioSources[1];
            _carMotorAudioGoing = AudioSources[2];
            
            _offroadWarningAudio = AudioSources[3];
            _teleportBackAudio = AudioSources[4];

            _jumpAudio = AudioSources[5];
            _gearShiftAudio = AudioSources[6];

            FirstPlaceAudio = AudioSources[7];
            SecondPlaceAudio = AudioSources[8];
            ThirdPlaceAudio = AudioSources[9];
            FourthPlaceAudio = AudioSources[10];
            RoundTwo = AudioSources[11];
            FinalLapAudio = AudioSources[12];

            _collisionAudio = AudioSources[13];

            _landingAudio = AudioSources[14];
            BrakingAudio = AudioSources[15];

            CollisionWarningAudio = AudioSources[16];

        }
        
    }

    public virtual void MoveLogic()
    {
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Road"))
        {
            if (_isJumping)
            {
                _landingAudio.Play();
            }
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
            if (_isJumping)
            {
                _landingAudio.Play();
            }
            
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
        else if (other.gameObject.CompareTag("ShortCut"))
        {
            if (_isJumping)
            {
                _landingAudio.Play();
            }
            
            _isGrounded = true;
            _isJumping = false;

            if (!speedReduced)
            {
                characterRef.characterAcceleration *= 0.8f;
                speedReduced = true;
                maxSpeed = 25;
            }
        } 
        else if (other.gameObject.CompareTag("Item Wall") || other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Opponent"))
        {
            _collisionAudio.Play();
            if(gameObject.transform.position.y - other.transform.position.y > 1)
            {
                _rb.AddForce(transform.up*_jumpingPower, ForceMode.Impulse);        
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
        _offroadWarningAudio.Play();
        yield return new WaitForSeconds(5);
        if (speedReduced)
        {
            transform.position = new Vector3(trackManagerRef.spline.Spline[characterRef.checkPointsReached].Position.x, trackManagerRef.spline.Spline[characterRef.checkPointsReached].Position.y, trackManagerRef.spline.Spline[characterRef.checkPointsReached].Position.z);
            _teleportBackAudio.Play();
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
