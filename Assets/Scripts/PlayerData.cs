using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class PlayerData
{
    public bool beatenTrack1;
    public bool beatenTrack2;
    public bool beatenTrack3;
    public bool beatenTrack4;

    public PlayerData(GameManager gameManagerRef)
    {
        beatenTrack1 = gameManagerRef.beatenTrack1;
        beatenTrack2 = gameManagerRef.beatenTrack2;
        beatenTrack3 = gameManagerRef.beatenTrack3;
        beatenTrack4 = gameManagerRef.beatenTrack4;
    }
    
}
