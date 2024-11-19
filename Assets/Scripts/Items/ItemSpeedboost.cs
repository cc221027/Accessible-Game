using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpeedboost : ItemBase
{
    private VehicleBehaviour _vehicleRef;
    private CharacterData _characterRef;

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
        itemName = "Speedboost";
        PickUpTts = itemName;
        
        if (GameManager.Instance.toggleAccessibility)
        {
            UAP_AccessibilityManager.Say(gameObject.GetComponentInParent<CharacterData>().characterName + PickUpTts);
        }
    }
    public override void UseItem(GameObject player)
    {
        _vehicleRef = player.GetComponent<VehicleBehaviour>();
        _characterRef = player.GetComponent<CharacterData>();
        UseItemAudio.Play();
        StartCoroutine(SpeedBoost());
    }

    private IEnumerator SpeedBoost()
    {
        _vehicleRef.maxSpeed = 60;
        _characterRef.characterAcceleration *= 2;
        yield return new WaitForSeconds(1.5f);
        _vehicleRef.maxSpeed = 30;
        _characterRef.characterAcceleration = _characterRef.baseCharacterAcceleration;
        Destroy(gameObject);
    }
}
