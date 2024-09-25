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
        Debug.Log("Used item Wings");
        StartCoroutine(DoubleJump());
    }

    private IEnumerator DoubleJump()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}
