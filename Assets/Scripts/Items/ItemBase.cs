using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemBase : MonoBehaviour
{
    public string itemName;
    
    public float cooldownTime;
    private float _lastUsedTime;
    
    
    public virtual void OnPickup()
    {
        Debug.Log(itemName + " picked up!");
        // Additional logic like adding to inventory
    }
    
    public bool CanUseItem()
    {
        return Time.time >= _lastUsedTime + cooldownTime;
    }
    
    public virtual void UseItem()
    {
        Debug.Log(itemName + " used!");
    }
}
