using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWall : ItemBase
{
    // Start is called before the first frame update
    void Start()
    {
        itemName = "Wall";
    }

    public override void UseItem(GameObject player)
    {
        transform.parent = null;
        transform.position = (player.transform.position - (player.transform.forward * 5));
        transform.localScale = new Vector3(6, 3, 1);
    }

    private void OnCollisionEnter(Collision other)
    {
        BumpIntoPlayer(other.gameObject);
    }

    private void BumpIntoPlayer(GameObject player)
    {
        Destroy(gameObject);
    }
    
    
}
