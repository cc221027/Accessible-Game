using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class ItemOfficerJenkins : ItemBase
{
    private Rigidbody _rb;
    private bool _hasHitTarget;

    public Transform characterInSight = null;

    public bool shot = false;
    
    private Transform _targetPlayer;
    private AudioSource _bulletTravelAudio;
    private AudioSource _alarmPlayerAudio;
    private AudioSource _shootIndicatorAudio;

    private void Awake()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 2)
        {
            PickupAudioSource = audioSources[0];
            UseItemAudio = audioSources[1];
            _bulletTravelAudio = audioSources[2];
            _alarmPlayerAudio = audioSources[3];
            _shootIndicatorAudio = audioSources[4];
        }  
    }

    private void Start()
    {
        itemName = "SpecialBullet";
        PickUpTts = itemName;
        
        if (GameManager.Instance.toggleAccessibility)
        {
            UAP_AccessibilityManager.Say(gameObject.GetComponentInParent<CharacterData>().characterName + " " + PickUpTts);
        }
    }
    
    private void Update()
    {
        if (shot && 
            FindClosestPlayerPosition().gameObject.CompareTag("Player") && 
            Vector3.Distance(transform.position, FindClosestPlayerPosition().position) < 10 && 
            !_alarmPlayerAudio.isPlaying)
        {
            Vector3 toPlayer = (FindClosestPlayerPosition().position - transform.position).normalized;

            if (Vector3.Dot(transform.forward, toPlayer) > 0.5)
            {
                _alarmPlayerAudio.Play();
            }
        }

        if (Vector3.Distance(transform.position, FindClosestOpponent().position) < 20 && 
            !_shootIndicatorAudio.isPlaying)
        {
            Vector3 toOpponent = (FindClosestOpponent().position - transform.position).normalized;
            if (Vector3.Dot(transform.forward, toOpponent) > 0.5)
            {
                if (transform.parent != null && transform.parent.CompareTag("Player"))
                {
                    _shootIndicatorAudio.Play();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (shot)
        {
            Vector3 direction;
            
            if (characterInSight != null)
            {
                direction = (characterInSight.position - transform.position).normalized;
            }
            else 
            {
                FindClosestPlayer();
                Vector3 targetPosition = TrackManager.Instance.spline.Spline[ReturnNextKnot()].Position;
                direction = (targetPosition - transform.position).normalized;
            }

           
            transform.position += transform.forward;
            

            Vector3 flatDirection = new Vector3(direction.x, 0, direction.z);

            if (flatDirection.magnitude > 0)
            {
                Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5);
            }
        }
       
    }
    
    private int ReturnNextKnot()
    {
        int closestIndex = TrackManager.Instance.spline.Spline.IndexOf(TrackManager.Instance.spline.Spline.OrderBy(p => Vector3.Distance(transform.position, p.Position)).First());
        return (closestIndex + 1) % TrackManager.Instance.spline.Spline.Count;
    }

    public override void UseItem(GameObject player)
    {
        transform.parent = null;
        transform.position = (player.transform.position + player.transform.forward * 4 + player.transform.up);
        _rb = gameObject.AddComponent<Rigidbody>();
        _rb.useGravity = false;
        StartCoroutine(Shoot(player));
        shot = true;
        
        UseItemAudio.Play();

    }
    
    private void OnTriggerEnter(Collider other)
    {
        CharacterData otherCharacter = other.gameObject.GetComponent<CharacterData>();

        if (otherCharacter != null && shot && otherCharacter.characterName != "Officer Jenkins" && otherCharacter.status != "Invisible" )
        {
            otherCharacter.GetComponent<VehicleBehaviour>().bulletHitAudio.Play();
            StartCoroutine(SlowCharacterOnHit(otherCharacter)); 
        }        
    }


    private IEnumerator Shoot(GameObject player)
    {
        transform.position += player.transform.forward; 
        float waitTime = 100f;
        float elapsedTime = 0f;

        while (elapsedTime < waitTime && !_hasHitTarget)
        {
            yield return null; 
            elapsedTime += Time.deltaTime;
            AdjustPitchBasedOnDistance();
        }

        if (!_hasHitTarget)
        {
            Destroy(gameObject); 
        }
    }



    private IEnumerator SlowCharacterOnHit(CharacterData otherCharacter)
    {
        _hasHitTarget = true;
        otherCharacter.characterAcceleration *= 0.5f;
        yield return new WaitForSeconds(4);
        otherCharacter.characterAcceleration = otherCharacter.baseCharacterAcceleration;
        Destroy(gameObject);
    }
    
    private void FindClosestPlayer()
    {
        CharacterData[] players = FindObjectsOfType<CharacterData>();
        
        Transform closestPlayer = players
            .Where(player =>
                Vector3.Distance(transform.position, player.transform.position) > 0 && 
                Vector3.Dot(transform.forward, (player.transform.position - transform.position).normalized) > 0.9)
            .OrderBy(player => Vector3.Distance(transform.position, player.transform.position))
            .Select(player => player.transform)
            .FirstOrDefault();
        
        if (closestPlayer != null)
        {
            CharacterData otherCharacter = closestPlayer.gameObject.GetComponent<CharacterData>();

            if(Vector3.Distance(transform.position, new Vector3(closestPlayer.position.x, closestPlayer.position.y, closestPlayer.position.z)) < 70 &&
               otherCharacter.status != "Invisible" &&
               otherCharacter.characterName != "Officer Jenkins")
            {
                characterInSight = closestPlayer;
            }
        }
    }
    
    private Transform FindClosestPlayerPosition()
    {
        CharacterData[] players = FindObjectsOfType<CharacterData>();
        Transform closestPlayer = null;
        float minDistance = Mathf.Infinity;

        foreach (CharacterData player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestPlayer = player.transform;
            }
        }
        return closestPlayer;
    }
    
    private Transform FindClosestOpponent()
    {
        CompetitorsBehaviour[] opponents = FindObjectsOfType<CompetitorsBehaviour>();
        Transform closestOpponent = null;
        float minDistance = Mathf.Infinity;

        foreach (CompetitorsBehaviour opponent in opponents)
        {
            float distance = Vector3.Distance(transform.position, opponent.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestOpponent = opponent.transform;
            }
        }
        return closestOpponent;
    }
    
    private void AdjustPitchBasedOnDistance()
    {
        if (_targetPlayer == null) return;

        float distance = Vector3.Distance(transform.position, _targetPlayer.position);

        float pitch = Mathf.Lerp(1.5f, 0.5f, distance / 50f);

        _bulletTravelAudio.pitch = pitch;
    }
}
