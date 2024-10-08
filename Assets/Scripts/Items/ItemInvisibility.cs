using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ItemInvisibility : ItemBase
{
    private Renderer[] _characterRenderers;

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
        itemName = "Invisibility";
    }

    public override void UseItem(GameObject player)
    {
        _characterRenderers = player.GetComponentsInChildren<Renderer>();
        player.GetComponent<CharacterData>().status = "Invisible";
        
        StartCoroutine(MakeCharInvisible(player));
        
        UseItemAudio.Play();
    }

    private IEnumerator MakeCharInvisible(GameObject player)
    {
        foreach (var rend in _characterRenderers)
        {
            if (rend != null) 
            {
                rend.enabled = false;
            }
        }

        yield return new WaitForSeconds(5);

        foreach (var rend in _characterRenderers)
        {
            if (rend != null)
            {
                rend.enabled = true;
            }
        }

        player.GetComponent<CharacterData>().status = "";

        Destroy(gameObject);
    }
}