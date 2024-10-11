using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{
    private CompetitorsBehaviour _compBehaviourRef;

    void Start()
    {
        _compBehaviourRef = GetComponentInParent<CompetitorsBehaviour>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item") && Vector3.Dot(transform.forward, (other.transform.position - transform.position).normalized) > 0.9f && _compBehaviourRef.inventoryItem == null)
        {
                if (_compBehaviourRef.itemInSight == null)
                {
                    _compBehaviourRef.itemInSight = other.transform;
                }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if ((other.CompareTag("Item Wall") && Vector3.Dot(transform.forward, (other.transform.position - transform.position).normalized) > 0 && Vector3.Distance(transform.position, other.transform.position) <= 10)
            || (other.CompareTag("Obstacle") && Vector3.Distance(transform.position, other.transform.position) <= 20 && Vector3.Dot(transform.forward, (other.transform.position - transform.position).normalized) > 0.5f))
        {
            _compBehaviourRef.Jump();
        }

        if (other.CompareTag("Road Corner"))
        {
            _compBehaviourRef.isNearCorner = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_compBehaviourRef != null)
        {
            if (other.CompareTag("Road Corner"))
            {
                _compBehaviourRef.isNearCorner = false;
            }
        }
    }
}
