using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemJimmy : ItemBase
{

    private AudioSource _absorbItemAudio;
    private void Awake()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 2)
        {
            PickupAudioSource = audioSources[0];
            UseItemAudio = audioSources[1];
            _absorbItemAudio = audioSources[2];
        }  
    }

    private void Start()
    {
        itemName = "Shield";
        PickUpTts = itemName;
        
        if (GameManager.Instance.toggleAccessibility)
        {
            UAP_AccessibilityManager.Say(gameObject.GetComponentInParent<CharacterData>().characterName + " " +PickUpTts);
        }
    }

    public override void UseItem(GameObject player)
    {
        transform.position = (player.transform.position + transform.up + transform.forward);
        transform.localScale = new Vector3(4,4,4);
        StartCoroutine(Shield());
        
        UseItemAudio.Play();
    }

    private IEnumerator Shield()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        ItemBase otherItem = other.GetComponent<ItemBase>();

        if (otherItem != null && (otherItem.itemName is "Wall" or "Bullet" or "SpecialBullet"))
        {
            _absorbItemAudio.Play();
           Destroy(other.gameObject);
           StartCoroutine(AbsorbItem());
        }
    }

    private IEnumerator AbsorbItem()
    {
        yield return new WaitWhile(() => _absorbItemAudio.isPlaying);
        Destroy(gameObject);
    }

}
