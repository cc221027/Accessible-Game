using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRussel : ItemBase
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
        itemName = "Hourglass";
    }
    
    public override void UseItem(GameObject player)
    {
        Debug.Log("Used item Hourglass");
        StartCoroutine(TimeStop());
    }

    private IEnumerator TimeStop()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}
