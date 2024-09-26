using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemJimmy : ItemBase
{
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
        itemName = "Shield";
    }
    
    public override void UseItem(GameObject player)
    {
        transform.position = (player.transform.position + transform.up + transform.forward);
        transform.localScale = new Vector3(4,4,4);
        StartCoroutine(Shield());
    }

    private IEnumerator Shield()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        ItemBase otherItem = other.GetComponent<ItemBase>();

        if (otherItem != null && otherItem.itemName == "Wall")
        {
           Destroy(other.gameObject);
           Destroy(gameObject);
        }
    }
}
