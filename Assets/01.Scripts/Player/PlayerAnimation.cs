using Unity.Netcode;
using UnityEngine;
using MoreMountains.Feedbacks;

public class PlayerAnimation : NetworkBehaviour
{
    private Animator _animator;
    private MMF_Player _MMF_Player;

    private readonly string Eye_Attack = "Attack";
    private readonly string Eye_Hurt = "Hurt";

    public override void OnNetworkSpawn()
    {
      
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _MMF_Player = GetComponentInChildren<MMF_Player>();
    }

    [ServerRpc]
    public void AttackAnimationServerRpc()
    {
        _animator.SetTrigger(Eye_Attack);
    }

    [ClientRpc]
    public void AttackAnimationClientRpc()
    {
        _animator.SetTrigger(Eye_Attack);
        if(IsOwner)
            AttackAnimationServerRpc();
    }

    [ServerRpc]
    public void HurtAnimationServerRpc()
    {
        _animator.SetTrigger(Eye_Hurt);
    }

    [ClientRpc]
    public void HurtAnimationClientRpc()
    {
        _animator.SetTrigger(Eye_Hurt);
        if (IsOwner)
            _MMF_Player?.PlayFeedbacks();
        if (IsOwner)
            HurtAnimationServerRpc();
    }
}