using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{
    private CompetitorsBehaviour _compBehaviourRef;
    
    private BoxCollider _boxCollider;

    void Start()
    {
        _compBehaviourRef = GetComponentInParent<CompetitorsBehaviour>();
        _boxCollider = GetComponent<BoxCollider>();
    }
    void OnDrawGizmos()
    {
        if (_boxCollider != null)
        {
            Gizmos.color = Color.green;

            // Calculate the world position and size of the collider
            Vector3 colliderWorldPosition = _boxCollider.transform.TransformPoint(_boxCollider.center);
            Vector3 colliderWorldSize = _boxCollider.size;

            // Draw the wireframe cube
            Gizmos.DrawWireCube(colliderWorldPosition, colliderWorldSize);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            _compBehaviourRef.itemInSight = other.transform;
            _compBehaviourRef.foundItem = true;
        }

        if (other.CompareTag("Item Wall"))
        {
            _compBehaviourRef.Jump();
        }
        if (other.CompareTag("Road Corner"))
        {
            _compBehaviourRef.SlowDown();
        }
    }
}
