using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBullet : ItemBase
{
    private Rigidbody _rb;
    private Collider _bulletCollider;
    private Renderer _bulletRenderer;
    private Renderer _bulletChildRenderer;
    private bool _hasHitTarget;
    private bool _shot;

    private void Awake()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 2)
        {
            PickupAudioSource = audioSources[0];
            UseItemAudio = audioSources[1];
        }  
    }

    void Start()
    {
        itemName = "Bullet";
        _bulletRenderer = transform.GetChild(0).gameObject.GetComponent<Renderer>();
        _bulletChildRenderer = _bulletRenderer.gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>();
    }
    
    public override void UseItem(GameObject player)
    {
        transform.parent = null;
        transform.position = (player.transform.position + player.transform.forward * 4 + player.transform.up);
        _rb = gameObject.AddComponent<Rigidbody>();
        _rb.useGravity = false;
        _bulletCollider = gameObject.AddComponent<SphereCollider>();
        StartCoroutine(Shoot(player));
        _shot = true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        CharacterData otherCharacter = other.gameObject.GetComponent<CharacterData>();
        
        if (otherCharacter != null && _shot)
        {
            _bulletCollider.enabled = false;
            _bulletRenderer.enabled = false;
            _bulletChildRenderer.enabled = false;
            StartCoroutine(SlowCharacterOnHit(otherCharacter)); 
        }
        else
        {
            ItemBase otherItem = other.GetComponent<ItemBase>();

            if (otherItem != null && (otherItem.itemName is "Wall" or "Bullet" or "SpecialBullet"))
            {
                Destroy(other.gameObject);
                Destroy(gameObject);
            }
        }     
        
        
    }


    private IEnumerator Shoot(GameObject player)
    {
        _rb.AddForce(player.transform.forward * 40, ForceMode.Impulse); 
        
        float waitTime = 5f;
        float elapsedTime = 0f;

        while (elapsedTime < waitTime && !_hasHitTarget)
        {
            yield return null; 
            elapsedTime += Time.deltaTime; 
        }

        if (!_hasHitTarget)
        {
            Destroy(gameObject); 
        }
    }



    private IEnumerator SlowCharacterOnHit(CharacterData otherCharacter)
    {
        otherCharacter.characterAcceleration *= 0.5f;
        yield return new WaitForSeconds(4);
        otherCharacter.characterAcceleration = otherCharacter.baseCharacterAcceleration;
        Destroy(gameObject);
    }
}
