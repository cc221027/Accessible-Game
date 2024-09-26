using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStella : ItemBase
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
        itemName = "Wings";
    }
    
    public override void UseItem(GameObject player)
    {
        player.GetComponent<Rigidbody>().AddForce(player.transform.up * 15, ForceMode.Impulse);
        Destroy(gameObject);
    }

   
}
