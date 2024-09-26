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

        Debug.Log(otherCharacter.characterName);
        Debug.Log(_itemOfficerJenkinsRef.shot);
        
        if (_itemOfficerJenkinsRef.shot && otherCharacter.characterName != "Officer Jenkins" && (other.CompareTag("Opponent") || other.CompareTag("Player")) && Vector3.Dot(transform.up, (other.transform.position - transform.position).normalized) > 0.7f && _itemOfficerJenkinsRef.characterInSight == null)
        {
            if (_itemOfficerJenkinsRef.characterInSight == null)
            {
                _itemOfficerJenkinsRef.characterInSight = other.transform;
                Debug.Log(other.name);
            }
        }
    }
}
