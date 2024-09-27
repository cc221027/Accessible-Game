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

    private void OnTriggerEnter(Collider other)
    {
        CharacterData otherCharacter = other.gameObject.GetComponent<CharacterData>();
        if (otherCharacter != null)
        {
            if (otherCharacter.status != "Invisible" &&_itemOfficerJenkinsRef.shot && otherCharacter.characterName != "Officer Jenkins" && (other.CompareTag("Opponent") || other.CompareTag("Player")) && (_itemOfficerJenkinsRef.characterInSight == null))
            {
                _itemOfficerJenkinsRef.characterInSight = other.transform;
            }
        }
        
    }
}
