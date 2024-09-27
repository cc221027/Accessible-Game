using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ItemInvisibility : ItemBase
{
    private List<Renderer> _characterRenderers = new List<Renderer>();

    private void Awake()
    {
        itemName = "Invisibility";
        transform.rotation = quaternion.RotateY(90);
    }

    public override void UseItem(GameObject player)
    {
        _characterRenderers.AddRange(player.GetComponentsInChildren<Renderer>());
        player.GetComponent<CharacterData>().status = "Invisible";
        
        StartCoroutine(MakeCharInvisible(player));
    }

    private IEnumerator MakeCharInvisible(GameObject player)
    {
        foreach (var rend in _characterRenderers)
        {
            rend.enabled = false;
        }

        yield return new WaitForSeconds(5);

        foreach (var rend in _characterRenderers)
        {
            rend.enabled = true;
        }
        
        player.GetComponent<CharacterData>().status = "";
        
        Destroy(gameObject);
    }
}