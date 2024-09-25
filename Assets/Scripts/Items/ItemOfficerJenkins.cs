using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOfficerJenkins : ItemBase
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
        itemName = "SpecialBullet";
    }
    
    public override void UseItem(GameObject player)
    {
        Debug.Log("Used item SpecialBullet");
        StartCoroutine(Shoot());
    }

    private IEnumerator Shoot()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}
