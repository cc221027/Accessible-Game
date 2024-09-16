using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWall : ItemBase
{
    private AudioSource _wallCollisionAudio;
    private void Awake()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 2)
        {
            PickupAudioSource = audioSources[0];
            UseItemAudio = audioSources[1];
            _wallCollisionAudio = audioSources[2];
        }
        
    }

    void Start()
    {
        itemName = "Wall";
    }
    public override void UseItem(GameObject player)
    {
        transform.parent = null;
        transform.position = (player.transform.position - (player.transform.forward * 5));
        transform.localScale = new Vector3(6, 3, 1);
        UseItemAudio.Play();
    }

    private void OnCollisionEnter(Collision other)
    {
        _wallCollisionAudio.Play();
        StartCoroutine(DestroyAfterSound());
    }

    private IEnumerator DestroyAfterSound()
    {
        yield return new WaitUntil(() => !_wallCollisionAudio.isPlaying);
        Destroy(gameObject);
    }

    
    
}
