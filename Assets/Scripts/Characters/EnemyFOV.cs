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
    
}
