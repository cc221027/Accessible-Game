using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class ItemBase : MonoBehaviour
{
    public string itemName;
    
    protected float CooldownTime;
    private float _lastUsedTime;

    protected AudioSource PickupAudioSource;
    protected AudioSource UseItemAudio;

    public void OnPickup()
    {
        PickupAudioSource.Play();
    }
    
    public bool CanUseItem()
    {
        return Time.time >= _lastUsedTime + CooldownTime;
    }
    
    public virtual void UseItem(GameObject player)
    {
        
    }
}
