using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Haptics;
using UnityEngine.Serialization;
using UnityEngine.Splines;
using Debug = UnityEngine.Debug;

public class PlayerController : VehicleBehaviour
{
    private TrackManager _trackManager;

    private float _steerInputValue;
    private float _accelerationInputValue;
    private float _decelerateValue;
    private float _itemUseValue;

    
    private SplineContainer _currentSpline;
    public List<BezierKnot> checkedSplines;

    private bool _shortCutPulseEnabled;
    private bool _enteredShortcut;

    private float _distanceToShortcut;
    
    private int _previousPlacement = -1;

    private bool _playedRoundTwoAudio;
    private bool _playedFinalAudio;
    
    private float _rayLength = 2f;
    private float _leftMotorStrength = 0.1f;
    private float _rightMotorStrength = 0.2f;

    private GameObject _pausePanel;

    private void Start()
    {
        _currentSpline = trackManagerRef.spline;
        
        _trackManager = TrackManager.Instance;
        
        _pausePanel = GameObject.Find("Canvas/PausedPanel");
        _pausePanel.SetActive(false);
        
    }

    private void Update()
    {
        if (GameManager.Instance.toggleAccessibility)
        {
            if (trackManagerRef.shortcutSpline != null)
            {
                _distanceToShortcut = Vector3.Distance(transform.position, trackManagerRef.shortcutSpline.Spline[0].Position);
            }
            else
            {
                _distanceToShortcut = float.MaxValue;
            }
            
            if (movementEnabled && !_shortCutPulseEnabled && _distanceToShortcut >= 70 && !trackManagerRef.paused)
            {
                float playerKnotSide = GetKnotSide();
                
                if (playerKnotSide > 0 && _rb.velocity.magnitude > 1)
                {
                    if (!GameManager.Instance.toggleSteering)
                    {
                        Gamepad.current.SetMotorSpeeds(_leftMotorStrength * (GameManager.Instance.hapticsVolume/100), 0);
                    }
                    else
                    {
                        CarMotorAudioGoing.panStereo = -1;
                        Debug.Log("Else reached");
                    }
                }
                else if (playerKnotSide < 0 && _rb.velocity.magnitude > 1)
                {
                    if (!GameManager.Instance.toggleSteering)
                    {
                        Gamepad.current.SetMotorSpeeds(0, _rightMotorStrength * (GameManager.Instance.hapticsVolume/100));
                    }
                    else
                    {
                        CarMotorAudioGoing.panStereo = 1;
                    }
                }
                else
                {
                    if (!GameManager.Instance.toggleSteering)
                    {
                        Gamepad.current.SetMotorSpeeds(0, 0); 
                    }
                    else
                    {
                        CarMotorAudioGoing.panStereo = 0;
                    }
                }
            } 
            

            if (characterRef.placement != _previousPlacement && movementEnabled)
            {
                _previousPlacement = characterRef.placement;
                PlayPlacementAudio(characterRef.placement);
            }

            if (characterRef.completedLaps == 1 && !_playedRoundTwoAudio)
            {
                _playedRoundTwoAudio = true;
                RoundTwo.Play();
            }
            else if (characterRef.completedLaps == 2 && !_playedFinalAudio)
            {
                _playedFinalAudio = true;
                FinalLapAudio.Play();
            }
            
            GetClosestObstacleOnTrack();
            CheckForShortCut();
            AdjustHapticsBasedOnObstacles();
            PanToClosestItemBox();
        }
    }

   

    private int GetClosestKnotIndex(SplineContainer spline)
    {
        return spline.Spline.IndexOf(spline.Spline.OrderBy(p => Vector3.Distance(transform.position, p.Position)).First());
    }

    private Vector3 GetNextKnotPosition(int closestIndex)
    {
        BezierKnot nextKnot = _currentSpline.Spline[(closestIndex + 1) % _currentSpline.Spline.Count];
        return nextKnot.Position;
    }

    
    private float GetKnotSide()
    {
        int closestIndex = GetClosestKnotIndex(_currentSpline);
        Vector3 nextKnot = GetNextKnotPosition(closestIndex);
        
        Vector3 directionToNextKnot = (nextKnot - transform.position).normalized;
        
        float dotProduct = Vector3.Dot(transform.forward, directionToNextKnot);

        if (dotProduct < 0 && !WrongDirectionAudio.isPlaying && !_enteredShortcut)
        {
            WrongDirectionAudio.Play(); 
            UAP_AccessibilityManager.Say("Wrong Direction");
        }
        else if(dotProduct >= 0)
        {
            WrongDirectionAudio.Stop(); 
        }
        
        Vector3 cross = Vector3.Cross(directionToNextKnot, transform.forward);

        return cross.y; 
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
        if (movementEnabled)
        {
            _decelerateValue = value.Get<float>();
            BrakingAudio.Play();
        }
       
    }
    

    private void OnJump(InputValue value)
    {
        if (movementEnabled)
        {
            Jump();
        }
    }

    private void OnItemUse(InputValue value)
    {
        _itemUseValue = value.Get<float>();
        UseItem();
    }
    
    public void OnPause()
    {
        if (movementEnabled)
        {
            Gamepad.current.SetMotorSpeeds(0,0);
            _pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
        
    }

    public override void MoveLogic()
    {
        float decelerationForce = _decelerateValue * 20;
        float accelerationForce = _accelerationInputValue * 20 * characterRef.characterAcceleration;
        float currentSpeed = _rb.velocity.magnitude;
        
        foreach (var spline in _currentSpline.Spline)
        {
            Vector3 directionToSpline = (Vector3)spline.Position - transform.position;
            
            if (Vector3.Distance(transform.position, spline.Position) < 25 && !checkedSplines.Contains(spline))
            {
                float dotProduct = Vector3.Dot(transform.forward, directionToSpline.normalized);
        
                if (dotProduct > 0f)
                {
                    characterRef.ReachedCheckPoint();
                    checkedSplines.Add(spline);
                }
            }
        }


        _trackManager.currentPlayerSpeed = Mathf.RoundToInt(currentSpeed);

        if (_decelerateValue > 0 && currentSpeed <= maxSpeed)
        {
            _rb.AddForce(-_rb.velocity.normalized * Mathf.Min(decelerationForce, currentSpeed), ForceMode.Acceleration);
        }
        else if (currentSpeed <= maxSpeed) 
        { 
            _rb.velocity = Vector3.Lerp(_rb.velocity, transform.forward * (maxSpeed * accelerationForce), Time.fixedDeltaTime * 0.05f);
        }
        

        float rotationMultiplier = _isGrounded && !speedReduced ? 1.4f : 0.7f;
        float rotationAmount = _steerInputValue * 80f * rotationMultiplier * Time.fixedDeltaTime;

        transform.Rotate(0, rotationAmount, 0);


    }

    public override void UseItem()
    {
        if (_itemUseValue > 0 && inventoryItem != null)
        {
            inventoryItem.GetComponent<ItemBase>().UseItem(gameObject);
            inventoryItem = null;
        }
    }

    private void PlayPlacementAudio(int placement)
    {

        switch (placement)
        {
            case 1:
                UAP_AccessibilityManager.Say("First Place");
                break;
            case 2:
                UAP_AccessibilityManager.Say("Second Place");
                break;
            case 3:
                UAP_AccessibilityManager.Say("Third Place");
                break;
            case 4:
                UAP_AccessibilityManager.Say("Fourth Place");
                break;
            default:
                return;
        }
        
    }
    
    private void GetClosestObstacleOnTrack()
    {
        bool shouldJump = trackManagerRef.obstaclesOnTrackPositions
            .Where(item =>
                Vector3.Distance(transform.position, item.position) <= 40 &&
                Vector3.Dot(transform.forward, (item.position - transform.position).normalized) > 0.7)
            .OrderBy(item => Vector3.Distance(transform.position, item.position))
            .FirstOrDefault();

        if (shouldJump && !CollisionWarningAudio.isPlaying)
        {
            CollisionWarningAudio.Play();
        }
    }

    private void CheckForShortCut()
    {
        if (trackManagerRef.shortcutSpline != null)
        {

            if (_distanceToShortcut is > 5 and < 70 && !_enteredShortcut)
            {
                
                Vector3 nextKnot = trackManagerRef.shortcutSpline.Spline[0].Position;
                Vector3 directionToNextKnot = (nextKnot - transform.position).normalized;
                Vector3 cross = Vector3.Cross(directionToNextKnot, transform.forward);
                
                if(!_shortCutPulseEnabled)
                {
                    StartCoroutine(PulseMotorForShortCut(cross.y));
                }
            }
            if (_distanceToShortcut < 10)
            {
                _currentSpline = trackManagerRef.shortcutSpline;
                _enteredShortcut = true;
            }
            else if (Vector3.Distance(transform.position,trackManagerRef.spline.Spline[GetClosestKnotIndex(trackManagerRef.spline)].Position) < 30 && _enteredShortcut)
            {
                _currentSpline = trackManagerRef.spline;
                _enteredShortcut = false;
            }
        }
    }

    private IEnumerator PulseMotorForShortCut(float side)
    {
        _shortCutPulseEnabled = true;
        
        float proximityFactor = Mathf.InverseLerp(70f, 5f, _distanceToShortcut);

        float motorStrength = Mathf.Lerp(0.1f, 1f, proximityFactor);
        
        for (int i = 0; i < 5; i++)
        {
            if (side > 0 && _rb.velocity.magnitude > 1)
            {
                if (!GameManager.Instance.toggleSteering)
                {
                    Gamepad.current.SetMotorSpeeds(motorStrength * (GameManager.Instance.hapticsVolume / 100), 0);
                }
                else
                {
                    CarMotorAudioGoing.panStereo = -1;
                    UAP_AccessibilityManager.Say("Shortcut Left Side");
                }
            }
            else if (side < 0 && _rb.velocity.magnitude > 1)
            {
                if (!GameManager.Instance.toggleSteering)
                {
                    Gamepad.current.SetMotorSpeeds(0, motorStrength * (GameManager.Instance.hapticsVolume / 100));
                }
                else
                {
                    CarMotorAudioGoing.panStereo = 1;
                    UAP_AccessibilityManager.Say("Shortcut Right Side");
                }
            }
            else
            {
                if (!GameManager.Instance.toggleSteering)
                {
                    Gamepad.current.SetMotorSpeeds(0, 0); 
                }
                else
                {
                    CarMotorAudioGoing.panStereo = 0;
                }
            }

            yield return new WaitForSeconds(0.2f);

            Gamepad.current.SetMotorSpeeds(0, 0);

            yield return new WaitForSeconds(0.2f);
        }
        
        float playerKnotSide = GetKnotSide();
                
        if (playerKnotSide > 0 && _rb.velocity.magnitude > 1)
        {
            if (!GameManager.Instance.toggleSteering)
            {
                Gamepad.current.SetMotorSpeeds(_leftMotorStrength * (GameManager.Instance.hapticsVolume/100), 0); 
            }
            else
            {
                CarMotorAudioGoing.panStereo = -1;
                UAP_AccessibilityManager.Say("Shortcut Left Side");
            }
        }
        else if (playerKnotSide < 0 && _rb.velocity.magnitude > 1)
        {
            if (!GameManager.Instance.toggleSteering)
            {
                Gamepad.current.SetMotorSpeeds(0, _rightMotorStrength * (GameManager.Instance.hapticsVolume/100));  
            }
            else
            {
                CarMotorAudioGoing.panStereo = 1;
                UAP_AccessibilityManager.Say("Shortcut Right Side");
            }
        }
        else
        {
            if (!GameManager.Instance.toggleSteering)
            {
                Gamepad.current.SetMotorSpeeds(0, 0); 
            }
            else
            {
                CarMotorAudioGoing.panStereo = 0;
            }
        }
        
        yield return new WaitForSeconds(1f);
        
        _shortCutPulseEnabled = false;
    }
    
    
    private void AdjustHapticsBasedOnObstacles()
    {
        bool frontBlocked = Physics.Raycast(transform.position, transform.forward, _rayLength);
        bool backBlocked = Physics.Raycast(transform.position, -transform.right, _rayLength);
        bool leftBlocked = Physics.Raycast(transform.position, -transform.right, _rayLength);
        bool rightBlocked = Physics.Raycast(transform.position, transform.right, _rayLength);

        if (frontBlocked || backBlocked)
        {
            _rayLength = 4;
        }
        else
        {
            _rayLength = 2;
        }
        
        if (leftBlocked)
        {
            _rightMotorStrength = 1f;
            _leftMotorStrength = 0.1f;
        }
        else if (rightBlocked)
        {
            _leftMotorStrength = 1f;
            _rightMotorStrength = 0.2f;
        }
        else
        {
            _leftMotorStrength = 0.1f;
            _rightMotorStrength = 0.2f;
        }
    }
    
    private void PanToClosestItemBox()
    {
        if (GameManager.Instance.toggleAccessibility)
        {
            Transform closestItemBox = trackManagerRef.itemBoxPositions
                .Where(itemBox =>
                    Vector3.Distance(transform.position, itemBox.position) >= 5 &&
                    Vector3.Distance(transform.position, itemBox.position) <= 140 &&
                    Vector3.Dot(transform.forward, (itemBox.position - transform.position).normalized) > 0.5)
                .OrderBy(item => Vector3.Distance(transform.position, item.position))
                .FirstOrDefault();


            if (closestItemBox != null)
            {
                Vector3 directionToNextKnot = (new Vector3(closestItemBox.position.x, closestItemBox.position.y, closestItemBox.position.z) - transform.position).normalized;
                Vector3 cross = Vector3.Cross(directionToNextKnot, transform.forward);

                if(cross.y > 0 && !trackManagerRef.paused)
                {
                    if (!GameManager.Instance.toggleSteering)
                    {
                        CarMotorAudioGoing.panStereo = -1;
                    }
                    else
                    {
                        Gamepad.current.SetMotorSpeeds(_leftMotorStrength * (GameManager.Instance.hapticsVolume/100), 0); 
                    }
                } 
                else if(cross.y < 0 && !trackManagerRef.paused)
                {
                    if (!GameManager.Instance.toggleSteering)
                    {
                        CarMotorAudioGoing.panStereo = 1;
                    }
                    else
                    {
                        Gamepad.current.SetMotorSpeeds(0, _rightMotorStrength * (GameManager.Instance.hapticsVolume/100));  
                    }
                }
            }
            else
            {
                if (!GameManager.Instance.toggleSteering)
                {
                    CarMotorAudioGoing.panStereo = 0;
                }                   
                else
                {
                    Gamepad.current.SetMotorSpeeds(0, 0);  

                }
            }
        }
    }
}
