using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemBase : MonoBehaviour
{
    public string itemName;
    
    public float cooldownTime;
    private float _lastUsedTime;
    
    
    public void OnPickup()
    {
    }
    
    public bool CanUseItem()
    {
        return Time.time >= _lastUsedTime + cooldownTime;
    }
    
    public virtual void UseItem(GameObject player)
    {
        
    }
}
