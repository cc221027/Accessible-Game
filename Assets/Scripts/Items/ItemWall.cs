using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWall : ItemBase
{
    private void Awake()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 2)
        {
            PickupAudioSource = audioSources[0];
            UseItemAudio = audioSources[1];
        }
        
    }
    
    private void Start()
    {        
        itemName = "Wall";
        PickUpTts = itemName;
        
        if (GameManager.Instance.toggleAccessibility)
        {
            UAP_AccessibilityManager.Say(gameObject.GetComponentInParent<CharacterData>().characterName + " " + PickUpTts);
        }
    }
    public override void UseItem(GameObject player)
    {
        transform.parent = null;
        transform.position = (player.transform.position - (player.transform.forward * 5) + (player.transform.up));
        TrackManager.Instance.obstaclesOnTrackPositions.Add(gameObject.transform);
        transform.localScale = new Vector3(6, 3, 1);
        gameObject.AddComponent<BoxCollider>();
        UseItemAudio.Play();
    }

    private void OnCollisionEnter()
    {
        TrackManager.Instance.obstaclesOnTrackPositions.Remove(gameObject.transform);
        Destroy(gameObject);
    }

    
    
}
