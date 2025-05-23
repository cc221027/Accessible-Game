using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBullet : ItemBase
{
    private Rigidbody _rb;
    private Collider _bulletCollider;
    private Renderer _bulletRenderer;
    private Renderer _bulletChildRenderer;
    private bool _hasHitTarget;
    private bool _shot;
    
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

    void Start()
    {
        itemName = "Bullet";
        PickUpTts = itemName;
        
        if (GameManager.Instance.toggleAccessibility)
        {
            UAP_AccessibilityManager.Say(gameObject.GetComponentInParent<CharacterData>().characterName + " " + PickUpTts);
        }
        
        _bulletRenderer = transform.GetChild(0).gameObject.GetComponent<Renderer>();
        _bulletChildRenderer = _bulletRenderer.gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>();
    }

    private void Update()
    {
        if (_shot && 
            FindClosestPlayer().gameObject.CompareTag("Player") && 
            Vector3.Distance(transform.position, FindClosestPlayer().position) < 10 && 
            !_alarmPlayerAudio.isPlaying)
        {
            Vector3 toPlayer = (FindClosestPlayer().position - transform.position).normalized;

            if (Vector3.Dot(transform.forward, toPlayer) > 0.5)
            {
                _alarmPlayerAudio.Play();
            }
        }

        if (FindClosestOpponent() != null && Vector3.Distance(transform.position, FindClosestOpponent().position) < 20 && 
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

    public override void UseItem(GameObject player)
    {
        transform.parent = null;
        transform.position = (player.transform.position + player.transform.forward * 4 + player.transform.up);
        _rb = gameObject.AddComponent<Rigidbody>();
        _rb.useGravity = false;
        _bulletCollider = gameObject.AddComponent<SphereCollider>();
        StartCoroutine(Shoot(player));
        _shot = true;
        
        UseItemAudio.Play();
        _bulletTravelAudio.Play();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        CharacterData otherCharacter = other.gameObject.GetComponent<CharacterData>();
        
        if (otherCharacter != null && _shot)
        {
            otherCharacter.GetComponent<VehicleBehaviour>().bulletHitAudio.Play();
            
            _hasHitTarget = true;

            _bulletCollider.enabled = false;
            _bulletRenderer.enabled = false;
            _bulletChildRenderer.enabled = false;
            StartCoroutine(SlowCharacterOnHit(otherCharacter));
        }
        else
        {
            ItemBase otherItem = other.GetComponent<ItemBase>();

            if (otherItem != null && (otherItem.itemName is "Wall" or "Bullet" or "SpecialBullet"))
            {
                _hasHitTarget = true;
                
                Destroy(other.gameObject);
                Destroy(gameObject);
            }
        }     
    }


    private IEnumerator Shoot(GameObject player)
    {
        _rb.AddForce(player.transform.forward * 40, ForceMode.Impulse); 
        
        float waitTime = 5f;
        float elapsedTime = 0f;

        while (elapsedTime < waitTime && !_hasHitTarget)
        {
            yield return null; 
            elapsedTime += Time.deltaTime; 
            _targetPlayer = FindClosestPlayer();
            AdjustPitchBasedOnDistance();
        }

        if (!_hasHitTarget)
        {
            Destroy(gameObject); 
        }
    }



    private IEnumerator SlowCharacterOnHit(CharacterData otherCharacter)
    {
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
    
    private Transform FindClosestOpponent()
    {
        CompetitorsBehaviour[] opponents = FindObjectsOfType<CompetitorsBehaviour>();
        if (opponents != null)
        {
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

        return null;
    }
    
    private void AdjustPitchBasedOnDistance()
    {
        if (_targetPlayer == null) return;

        float distance = Vector3.Distance(transform.position, _targetPlayer.position);

        float pitch = Mathf.Lerp(1.5f, 0.5f, distance / 50f);

        _bulletTravelAudio.pitch = pitch;
    }
}
