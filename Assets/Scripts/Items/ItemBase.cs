using System;
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

    private void Update()
    {
        PickupAudioSource.volume = GameManager.Instance.sfxVolume / 100;
        UseItemAudio.volume = GameManager.Instance.sfxVolume / 100;
    }

    public bool CanUseItem()
    {
        return Time.time >= _lastUsedTime + CooldownTime;
    }
    
    public virtual void UseItem(GameObject player)
    {
        
    }
}
