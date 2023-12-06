using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Knife : NetworkBehaviour
{
    [SerializeField] private PlayerAiming _playerAiming;
    [SerializeField] private int _damage = 20;
    private NetworkVariable<int> knifeSocre = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private float stretchFactor = 1f;

    public void AddknifeSocre(int socre)
    {
        ModifyKnifeSocre(socre);
    }

    private void Update()
    {
        if (!IsOwner)
            return;
        transform.localRotation = Quaternion.Euler(0, 90, 0);
    }

    private void ModifyKnifeSocre(int socre)
    {
        knifeSocre.Value = Mathf.Clamp(knifeSocre.Value + socre, 0, 250);
        SetKnifeScaleClientRpc();
    }


    [ClientRpc]
    private void SetKnifeScaleClientRpc()
    {
        if (!IsOwner)
            return;

        // 현재 위치와 크기를 가져옵니다.
        Vector3 currentPosition = transform.position;
        Vector3 currentScale = transform.localScale;

        // y 축의 길이만 변경하고 다른 축은 고정시킵니다.
        currentScale.y *= Mathf.Pow(stretchFactor, knifeSocre.Value);
        currentScale.x = 1.0f; // x 축을 고정
        currentScale.z = 1.0f; // z 축을 고정

        // 길이가 음수가 되지 않도록 보정
        currentScale.y = Mathf.Max(0.1f, currentScale.y);

        // 스케일이 변경된 y 축 길이만큼 위치를 조정하여 오브젝트를 중앙에 유지
        currentPosition.y += Mathf.Clamp((currentScale.y - transform.localScale.y) * 0.5f, 1, 50);

        // 새로운 위치와 스케일을 적용합니다.
        transform.position = currentPosition;
        transform.localScale = currentScale;
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
