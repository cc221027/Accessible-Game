using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racer2 : CharacterData
{
    // Start is called before the first frame update
    void Start()
    {
        index = 1;
        characterName = "Officer Jenkins";
        baseCharacterAcceleration = 1.2f;
        characterAcceleration = baseCharacterAcceleration;

    }

}
