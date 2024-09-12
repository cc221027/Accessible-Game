using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemPickupContainer : MonoBehaviour
{
    [SerializeField] private List<GameObject> items = new List<GameObject>();
    
    private Renderer[] _renderers;
    private Collider[] _colliders;

    private AudioSource _pickupContainerHoveringAudio;

    private void Awake()
    {
        // Cache the Renderer and Collider components
        _renderers = GetComponentsInChildren<Renderer>();
        _colliders = GetComponentsInChildren<Collider>();
    }

    private void Start()
    {
        _pickupContainerHoveringAudio = GetComponent<AudioSource>();
        _pickupContainerHoveringAudio.loop = true;
        //_pickupContainerHoveringAudio.Play();
    }

    public void GetRandomItem(GameObject player)
    {
        GameObject item = Instantiate(items[Random.Range(0, items.Count)], player.transform.position + (player.transform.right * 2) + player.transform.up, player.transform.rotation);
        item.transform.SetParent(player.transform);
        item.GetComponent<ItemBase>().OnPickup();
        player.GetComponent<VehicleBehaviour>().inventoryItem = item;
        
        StartCoroutine(ResetItemBox());
    }

    private IEnumerator ResetItemBox()
    {
        foreach (var visuals in _renderers) { visuals.enabled = false; }
        foreach (var coll in _colliders) { coll.enabled = false; }
        
        yield return new WaitForSeconds(3);
        
        foreach (var visuals in _renderers) { visuals.enabled = true; }
        foreach (var coll in _colliders) { coll.enabled = true; }

    }
}
