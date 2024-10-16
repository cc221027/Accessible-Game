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
    private float _driftValue;
    private float _itemUseValue;

    private bool _isDrifting;
    private bool _driftEnded;
    private float _driftSteerLock;

    private PlayerInput _playerInput;
    
    private SplineContainer _currentSpline;
    public List<BezierKnot> checkedSplines;

    private bool _shortCutPulseEnabled;

    private float _rangeToLeftOffroad;
    private float _rangeToRightOffroad;
    
    private int _previousPlacement = -1;

    private bool _playedRoundTwoAudio;
    private bool _playedFinalAudio;

    private GameObject _pausePanel;

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _currentSpline = trackManagerRef.spline;
        
        _trackManager = TrackManager.Instance;
        
        _pausePanel = GameObject.Find("Canvas/PausedPanel");
        _pausePanel.SetActive(false);
        
    }

    private void Update()
    {
        if (GameManager.Instance.toggleAccessibility)
        {
            float playerKnotSide = GetKnotSide();

            if (movementEnabled && !_shortCutPulseEnabled)
            {
                if (playerKnotSide >= 0.1)
                {
                    Gamepad.current.SetMotorSpeeds(0.2f * (GameManager.Instance.hapticsVolume/100), 0); 
                }
                else if (playerKnotSide <= -0.1)
                {
                    Gamepad.current.SetMotorSpeeds(0, 0.2f * (GameManager.Instance.hapticsVolume/100));  
                }
                else
                {
                    Gamepad.current.SetMotorSpeeds(0, 0); 
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

    private void OnDrift(InputValue value)
    {
        _driftValue = value.Get<float>();
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
            _pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
        
    }

    public override void MoveLogic()
    {
        float decelerationForce = _decelerateValue * 20;
        float accelerationForce = _accelerationInputValue * 20 * characterRef.characterAcceleration;
        float currentSpeed = _rb.velocity.magnitude;
        _isDrifting = _driftValue > 0;





        foreach (var spline in _currentSpline.Spline)
        {
            if (Vector3.Distance(transform.position, spline.Position) < 20 && !checkedSplines.Contains(spline))
            {
                characterRef.ReachedCheckPoint();
                checkedSplines.Add(spline);
            }
        }

        _trackManager.currentPlayerSpeed = Mathf.RoundToInt(currentSpeed);

        if (_isDrifting && _isGrounded && currentSpeed >= 10 && !speedReduced)
        {
            if (_steerInputValue != 0)
            {
                Vector3 targetVelocity = transform.forward * accelerationForce;
                _rb.velocity = Vector3.Lerp(_rb.velocity, targetVelocity * 5, Time.fixedDeltaTime);
            }


            if (_driftSteerLock == 0)
            {
                _driftSteerLock = _steerInputValue;
            }

            if (_driftSteerLock < 0)
            {
                _steerInputValue = Mathf.Lerp(_steerInputValue, Mathf.Clamp(_steerInputValue, -1f, -0.3f), 0.7f);

            }
            else if (_driftSteerLock > 0)
            {
                _steerInputValue = Mathf.Lerp(_steerInputValue, Mathf.Clamp(_steerInputValue, 0.3f, 1f), 0.7f);
            }
        }
        else
        {
            if (_driftSteerLock != 0)
            {
                _steerInputValue = _playerInput.actions["Steer"].ReadValue<float>();
            }

            _driftSteerLock = 0;

            if (_decelerateValue > 0 && currentSpeed <= maxSpeed)
            {
                _rb.AddForce(-_rb.velocity.normalized * Mathf.Min(decelerationForce, currentSpeed),
                    ForceMode.Acceleration);
            }
            else if (currentSpeed <= maxSpeed)
            {
                _rb.AddForce(transform.forward * accelerationForce, ForceMode.Acceleration);
            }
        }

        float rotationMultiplier = _isDrifting && _isGrounded && !speedReduced ? 1.4f : 0.7f;
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
                SecondPlaceAudio.Stop();
                ThirdPlaceAudio.Stop();
                FourthPlaceAudio.Stop();
                FirstPlaceAudio.Play();
                break;
            case 2:
                FirstPlaceAudio.Stop();
                ThirdPlaceAudio.Stop();
                FourthPlaceAudio.Stop();
                SecondPlaceAudio.Play();
                break;
            case 3:
                FirstPlaceAudio.Stop();
                SecondPlaceAudio.Stop();
                FourthPlaceAudio.Stop();
                ThirdPlaceAudio.Play();
                break;
            case 4:
                FirstPlaceAudio.Stop();
                SecondPlaceAudio.Stop();
                ThirdPlaceAudio.Stop();
                FourthPlaceAudio.Play();
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
            float distanceToShortcut = Vector3.Distance(transform.position, trackManagerRef.shortcutSpline.Spline[0].Position);

            if (distanceToShortcut is > 5 and < 70 && !_shortCutPulseEnabled)
            {
                
                Vector3 nextKnot = trackManagerRef.shortcutSpline.Spline[0].Position;
                Vector3 directionToNextKnot = (nextKnot - transform.position).normalized;
                Vector3 cross = Vector3.Cross(directionToNextKnot, transform.forward);

                StartCoroutine(PulseMotorForShortCut(cross.y));
            }
            if (distanceToShortcut < 5)
            {
                _currentSpline = trackManagerRef.shortcutSpline;
            }
            else if (Vector3.Distance(transform.position,trackManagerRef.spline.Spline[GetClosestKnotIndex(trackManagerRef.spline)].Position) < 25)
            {
                _currentSpline = trackManagerRef.spline;
            }
        }
    }

    private IEnumerator PulseMotorForShortCut(float side)
    {
        _shortCutPulseEnabled = true;
        
        if (side > 0)
        {
            Gamepad.current.SetMotorSpeeds(1f * (GameManager.Instance.hapticsVolume / 100), 0);
        }
        else if (side < 0) 
        {
            Gamepad.current.SetMotorSpeeds(0, 1f * (GameManager.Instance.hapticsVolume / 100));
        }

        yield return new WaitForSecondsRealtime(1f);

        Gamepad.current.SetMotorSpeeds(0, 0);

        yield return new WaitForSecondsRealtime(1f);
        
        _shortCutPulseEnabled = false;
    }
}
