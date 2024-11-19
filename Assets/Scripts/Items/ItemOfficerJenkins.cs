using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

public class ItemOfficerJenkins : ItemBase
{
    private Rigidbody _rb;
    private bool _hasHitTarget;

    public Transform characterInSight = null;

    public bool shot = false;
    
    private Transform _targetPlayer;
    private AudioSource _bulletTravelAudio;

    private void Awake()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 2)
        {
            PickupAudioSource = audioSources[0];
            UseItemAudio = audioSources[1];
            _bulletTravelAudio = audioSources[2];
        }  
    }

    private void Start()
    {
        itemName = "SpecialBullet";
        PickUpTts = itemName;
        
        if (GameManager.Instance.toggleAccessibility)
        {
            UAP_AccessibilityManager.Say(gameObject.GetComponentInParent<CharacterData>().characterName + PickUpTts);
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

        if (otherCharacter != null && shot && otherCharacter.characterName != "Officer Jenkins")
        {
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
            FindClosestPlayer();
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
    
    private Transform FindClosestPlayer()
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
    
    private void AdjustPitchBasedOnDistance()
    {
        if (_targetPlayer == null) return;

        float distance = Vector3.Distance(transform.position, _targetPlayer.position);

        float pitch = Mathf.Lerp(1.5f, 0.5f, distance / 50f);

        _bulletTravelAudio.pitch = pitch;
    }
}
