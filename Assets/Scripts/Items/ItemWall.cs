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

    void Start()
    {
        itemName = "Wall";
    }
    public override void UseItem(GameObject player)
    {
        transform.parent = null;
        transform.position = (player.transform.position - (player.transform.forward * 5) + (player.transform.up));
        transform.localScale = new Vector3(6, 3, 1);
        gameObject.AddComponent<BoxCollider>();
        UseItemAudio.Play();
    }

    private void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
    }

    
    
}
