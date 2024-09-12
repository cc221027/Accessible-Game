using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class ItemBase : MonoBehaviour
{
    public string itemName;
    
    public float cooldownTime;
    private float _lastUsedTime;

    public AudioSource pickupAudioSource;
    public AudioSource useItemAudio;    
    public void OnPickup()
    {
        pickupAudioSource.Play();
    }
    
    public bool CanUseItem()
    {
        return Time.time >= _lastUsedTime + cooldownTime;
    }
    
    public virtual void UseItem(GameObject player)
    {
        
    }
}
