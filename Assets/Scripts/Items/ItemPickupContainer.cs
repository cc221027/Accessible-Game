using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickupContainer : MonoBehaviour
{
    [SerializeField] private List<GameObject> items;
    public void GetRandomItem(Vector3 position, Quaternion rotation)
    {
        GameObject item = Instantiate(items[Random.Range(0, items.Count)], position, rotation);
        item.GetComponent<ItemBase>().OnPickup();
    }
}
