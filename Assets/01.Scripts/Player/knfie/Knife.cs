using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class Knife : NetworkBehaviour
{
    [SerializeField] private PlayerAnimation _playerAnimation;
    [SerializeField] private PlayerAiming _playerAiming;
    [SerializeField] private int _damage = 20;
    public NetworkVariable<int> knifeSocre = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private float stretchFactor = 1f;

    public void AddknifeSocre(int socre)
    {
        ModifyKnifeSocre(socre);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;
        knifeSocre.OnValueChanged += (int previousValue, int newValue) => 
        {
            RankBoardBehaviour.Instance.onUserScoreChanged?.Invoke(OwnerClientId, newValue);
        };
    }

    public override void OnNetworkDespawn()
    {
        knifeSocre.OnValueChanged -= (int previousValue, int newValue) =>
        {
            RankBoardBehaviour.Instance.onUserScoreChanged?.Invoke(OwnerClientId, newValue);
        };
    }

    private void Update()
    {
        if (!IsOwner)
            return;
        //transform.localRotation = Quaternion.Euler(0, 0, -90);
    }

    private void ModifyKnifeSocre(int socre)
    {
        knifeSocre.Value = Mathf.Clamp(knifeSocre.Value + socre, 0, 1000);
        SetKnifeScaleClientRpc();
    }


    [ClientRpc]
    private void SetKnifeScaleClientRpc()
    {
        if (!IsOwner)
            return;
        float scale = Mathf.Clamp(knifeSocre.Value * stretchFactor, 1, 5);

        if (scale <= 4)
            _playerAnimation.ChangeEyeCloseServerRpc();

        Vector3 newScale = new Vector3(scale, scale, transform.localScale.z);
        transform.localScale = newScale;

        // z로테이션을 -90으로 고정하기
        transform.rotation = Quaternion.Euler(0, 0, -90);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer)
            return;

        if (collision.TryGetComponent<Health>(out Health health))
            health.TakeDamage(_damage, OwnerClientId);

        if (collision.TryGetComponent<Knife>(out Knife knife))
        {
            knife._playerAiming.ChangeDir();
            DebugToClientRpc(OwnerClientId, knife.OwnerClientId, knife.GetComponentInParent<Player>().GetUserName());
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (!IsServer)
    //        return;
    //    if (collision.attachedRigidbody.TryGetComponent<Health>(out Health health))
    //        health.TakeDamage(_damage, OwnerClientId);

    //    if (collision.TryGetComponent<Knife>(out Knife knife))
    //    {
    //        knife._playerAiming.ChangeDir();
    //        DebugToClientRpc(OwnerClientId, knife.OwnerClientId, knife.GetComponentInParent<Player>().GetUserName());
    //    }

    //    if(collision.attachedRigidbody.TryGetComponent<PlayerMovement>(out PlayerMovement player))
    //    {
    //        //Vector3 inVec = collision.GetContacts(0)
    //    }
    //}

    [ClientRpc]
    private void DebugToClientRpc(ulong owner,ulong hit, FixedString32Bytes hitName)
    {
        if (OwnerClientId != owner || OwnerClientId != hit)
            return;

        Debug.Log($"{GetComponentInParent<Player>().GetUserName()} -> {hitName}");
    }
}
