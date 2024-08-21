using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racer1 : CharacterData
{
    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        characterName = "Racer1";
        baseCharacterAcceleration = 1f;
        characterWeight = 5f;
        characterAcceleration = baseCharacterAcceleration;
    }
    
}
