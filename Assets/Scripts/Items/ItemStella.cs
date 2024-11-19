using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStella : ItemBase
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
        itemName = "Wings";
        PickUpTts = itemName;
        
        if (GameManager.Instance.toggleAccessibility)
        {
            UAP_AccessibilityManager.Say(gameObject.GetComponentInParent<CharacterData>().characterName + " " + PickUpTts);
        }
    }
    
    public override void UseItem(GameObject player)
    {
        UseItemAudio.Play();

        player.GetComponent<Rigidbody>().AddForce(player.transform.up * 15, ForceMode.Impulse);
        Destroy(gameObject);
        
    }

   
}
