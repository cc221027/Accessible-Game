using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class ItemPickupContainer : MonoBehaviour
{
    [SerializeField] private List<GameObject> items = new List<GameObject>();
    [SerializeField] private List<GameObject> characterItems = new List<GameObject>();

    
    private Renderer[] _renderers;
    private Collider[] _colliders;

    private AudioSource _pickupContainerHoveringAudio;
    private AudioSource _itemBoxBreaking;

    private void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        _colliders = GetComponentsInChildren<Collider>();
    }

    private void Start()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();

        if (audioSources.Length >= 2)
        {
            _pickupContainerHoveringAudio = audioSources[0];
            _itemBoxBreaking = audioSources[1];
        }

        _pickupContainerHoveringAudio.loop = true;
        _pickupContainerHoveringAudio.Play();
    }

    public void GetRandomItem(GameObject player)
    {
        int randomItemNumb = Random.Range(0, items.Count + 1);
        Vector3 itemPosition = player.transform.position + (player.transform.right * 2) + player.transform.up;

        var item = Instantiate(randomItemNumb < items.Count ? items[randomItemNumb] : characterItems[player.GetComponent<CharacterData>().index], itemPosition, player.transform.rotation);

        item.transform.SetParent(player.transform);
        item.GetComponent<ItemBase>().OnPickup();
        player.GetComponent<VehicleBehaviour>().inventoryItem = item;
        
        StartCoroutine(ResetItemBox());
    }

    private IEnumerator ResetItemBox()
    {
        foreach (var visuals in _renderers) { visuals.enabled = false; }
        foreach (var coll in _colliders) { coll.enabled = false; }
        
        _pickupContainerHoveringAudio.Stop();
        _itemBoxBreaking.Play();
        yield return new WaitForSeconds(3);
        _pickupContainerHoveringAudio.Play();
        
        foreach (var visuals in _renderers) { visuals.enabled = true; }
        foreach (var coll in _colliders) { coll.enabled = true; }

    }
}
