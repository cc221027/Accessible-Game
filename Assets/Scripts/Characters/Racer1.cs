using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racer1 : CharacterData
{
    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        characterName = "Jimmy";
        baseCharacterAcceleration = 1f;
        characterAcceleration = baseCharacterAcceleration;
    }
}
