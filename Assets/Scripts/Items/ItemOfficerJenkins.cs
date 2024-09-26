using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class ItemOfficerJenkins : ItemBase
{
    private Rigidbody _rb;
    private Collider _bulletCollider;
    private Renderer _bulletRenderer;
    private bool _hasHitTarget;

    private int _maxSpeed = 40;
    public Transform characterInSight = null;

    private bool _shot = false;

    private void Awake()
    {
        // AudioSource[] audioSources = GetComponents<AudioSource>();
        // if (audioSources.Length >= 2)
        // {
        //     PickupAudioSource = audioSources[0];
        //     UseItemAudio = audioSources[1];
        // }  
    }

    void Start()
    {
        itemName = "SpecialBullet";
        _bulletRenderer = transform.GetChild(0).gameObject.GetComponent<Renderer>();    
    }

    private void FixedUpdate()
    {
        if (_shot)
        {
            Vector3 direction;
            
            if (characterInSight != null)
            {
                direction = (characterInSight.position - transform.position).normalized;
                Debug.Log("Locked in on character");
            }
            else 
            {
                Vector3 targetPosition = TrackManager.Instance.spline.Spline[ReturnNextKnot()].Position;
                
                direction = (targetPosition - transform.position).normalized;
                Debug.Log("Locked in on knot: " + ReturnNextKnot());
            }

            if (_rb.velocity.magnitude <= _maxSpeed)
            {
                _rb.AddForce(transform.forward, ForceMode.Acceleration);
            }

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
        return closestIndex + 1;
    }

    public override void UseItem(GameObject player)
    {
        transform.parent = null;
        transform.position = (player.transform.position + player.transform.forward * 4 + player.transform.up);
        transform.rotation = Quaternion.Euler(-90, player.transform.eulerAngles.y, player.transform.eulerAngles.z);
        _rb = gameObject.AddComponent<Rigidbody>();
        _rb.useGravity = false;
        _bulletCollider = gameObject.AddComponent<SphereCollider>();
        StartCoroutine(Shoot(player));
        _shot = true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        CharacterData otherCharacter = other.gameObject.GetComponent<CharacterData>();

        if (otherCharacter != null && _shot && otherCharacter.characterName != "Officer Jenkins")
        {
            _bulletCollider.enabled = false;
            _bulletRenderer.enabled = false;
            StartCoroutine(SlowCharacterOnHit(otherCharacter)); 
        }        
    }


    private IEnumerator Shoot(GameObject player)
    {
        _rb.AddForce(player.transform.forward * 40, ForceMode.Impulse); 
     
        float waitTime = 100f;
        float elapsedTime = 0f;

        while (elapsedTime < waitTime && !_hasHitTarget)
        {
            yield return null; 
            elapsedTime += Time.deltaTime; 
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
}
