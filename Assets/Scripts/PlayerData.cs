using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
[Serializable]
public class PlayerData
{
    public bool beatenTrack1;
    public bool beatenTrack2;
    public bool beatenTrack3;
    public bool beatenTrack4;
    
    public float allVolume;
    public float uiVolume;
    public float sfxVolume;
    public float musicVolume;
    public float ttsVolume;
    public float ttsSpeechRate;
    public float hapticsVolume;
    public bool toggleAccessibility;
    public bool toggleSteering;

    public PlayerData(GameManager gameManagerRef)
    {
        beatenTrack1 = gameManagerRef.beatenTrack1;
        beatenTrack2 = gameManagerRef.beatenTrack2;
        beatenTrack3 = gameManagerRef.beatenTrack3;
        beatenTrack4 = gameManagerRef.beatenTrack4;

        allVolume = gameManagerRef.allVolume;
        uiVolume = gameManagerRef.uiVolume;
        sfxVolume = gameManagerRef.sfxVolume;
        musicVolume = gameManagerRef.musicVolume;
        ttsVolume = gameManagerRef.ttsVolume;
        ttsSpeechRate = gameManagerRef.ttsSpeechRate;
        hapticsVolume = gameManagerRef.hapticsVolume;
        toggleAccessibility = gameManagerRef.toggleAccessibility;
        toggleSteering = gameManagerRef.toggleSteering;
    }
    
}
