using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Health : NetworkBehaviour
{
    public int maxHealth = 100;
    public NetworkVariable<int> currentHealth;

    private bool _isDead = false;
    
    public Action<Health> OnDie;

    public ulong LastHitDealerID { get; private set; }


    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        currentHealth.Value = maxHealth; //¼­¹ö¸¸
    }

    public override void OnNetworkDespawn()
    {

    }

    public void TakeDamage(int damageValue, ulong dealerID)
    {
        LastHitDealerID = dealerID;
        ModifyHealth(-damageValue);
    }

    public void RestoreHealth(int healValue)
    {
        ModifyHealth(healValue);
    }

    public void ModifyHealth(int value)
    {
        if (_isDead) return;

        currentHealth.Value = Mathf.Clamp(currentHealth.Value + value, 0, maxHealth);

        if(currentHealth.Value == 0)
        {
            OnDie?.Invoke(this);
            _isDead = true;
        }
    }
}
