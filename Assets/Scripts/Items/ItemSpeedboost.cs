using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpeedboost : ItemBase
{
    private VehicleBehaviour _vehicleRef;
    private CharacterData _characterRef;
    
    // Start is called before the first frame update
    void Start()
    {
        itemName = "Speedboost";
    }

    // Update is called once per frame
    public override void UseItem(GameObject player)
    {
        _vehicleRef = player.GetComponent<VehicleBehaviour>();
        _characterRef = player.GetComponent<CharacterData>();
        StartCoroutine(SpeedBoost());
    }

    private IEnumerator SpeedBoost()
    {
        _vehicleRef.maxSpeed = 65;
        _characterRef.characterAcceleration *= 2;
        yield return new WaitForSeconds(1.5f);
        _vehicleRef.maxSpeed = 50;
        _characterRef.characterAcceleration = _characterRef.baseCharacterAcceleration;
        Destroy(gameObject);
    }
}
