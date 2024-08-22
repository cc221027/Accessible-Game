using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpeedboost : ItemBase
{
    // Start is called before the first frame update
    void Start()
    {
        itemName = "Speedboost";
    }

    // Update is called once per frame
    public override void UseItem(GameObject player)
    {
        StartCoroutine(SpeedBoost(player));
    }

    private IEnumerator SpeedBoost(GameObject player)
    {
        player.GetComponent<CharacterData>().characterAcceleration *= 2;
        yield return new WaitForSeconds(1.5f);
        player.GetComponent<CharacterData>().characterAcceleration = player.GetComponent<CharacterData>().baseCharacterAcceleration;
    }
}
