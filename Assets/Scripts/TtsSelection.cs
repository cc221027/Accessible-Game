using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TtsSelection : MonoBehaviour, ISelectHandler
{
    public void OnSelect(BaseEventData data)
    {
        AudioClip clip = (AudioClip)Resources.Load("Audio/Menu/Navigation/Interact");
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(clip);
        audioSource.volume = 0.5f;
        
        gameObject.GetComponent<UAP_BaseElement>().SelectItem();
    }
}
