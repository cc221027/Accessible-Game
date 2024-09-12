using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWall : ItemBase
{
    // Start is called before the first frame update
    void Start()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 2)
        {
            pickupAudioSource = audioSources[0];
            useItemAudio = audioSources[1];
        }        
        itemName = "Wall";
    }

    public override void UseItem(GameObject player)
    {
        transform.parent = null;
        transform.position = (player.transform.position - (player.transform.forward * 5));
        transform.localScale = new Vector3(6, 3, 1);
        //useItemAudio.Play();
    }

    private void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
    }
    
    
}
