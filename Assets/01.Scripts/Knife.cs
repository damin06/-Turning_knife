using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Knife : NetworkBehaviour
{
    [SerializeField] private PlayerAiming _playerAiming;
    private int _damage;
    private ulong _ownerClientID;

    void Start()
    {
        if(IsOwner)
        _ownerClientID = OwnerClientId;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody.TryGetComponent<Health>(out Health health))
            health.TakeDamage(_damage, _ownerClientID);
        else if (collision.TryGetComponent<Knife>(out Knife knife))
            knife._playerAiming.ChangeDir();
    }
}
