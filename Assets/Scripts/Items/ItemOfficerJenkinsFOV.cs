using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemOfficerJenkinsFOV : MonoBehaviour
{
    private ItemOfficerJenkins _itemOfficerJenkinsRef;

    void Start()
    {
        _itemOfficerJenkinsRef = GetComponentInParent<ItemOfficerJenkins>();
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     CharacterData otherCharacter = other.gameObject.GetComponent<CharacterData>();
    //     
    //     Debug.Log("did you shoot? " +_itemOfficerJenkinsRef.shot);
    //     Debug.Log("is it officer jenkins? "+ otherCharacter.characterName != "Officer Jenkins");
    //     Debug.Log("is it either opponent or player? " + (other.CompareTag("Opponent") || other.CompareTag("Player")));
    //     Debug.Log("does the item already have a target? " + (_itemOfficerJenkinsRef.characterInSight == null));
    //     
    //     if (_itemOfficerJenkinsRef.shot && otherCharacter.characterName != "Officer Jenkins" && (other.CompareTag("Opponent") || other.CompareTag("Player")) && (_itemOfficerJenkinsRef.characterInSight == null))
    //     {
    //         _itemOfficerJenkinsRef.characterInSight = other.transform;
    //     }
    // }
}
