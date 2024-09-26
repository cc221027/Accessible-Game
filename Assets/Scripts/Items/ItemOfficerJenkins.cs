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
