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

        // ���� ��ġ�� ũ�⸦ �����ɴϴ�.
        Vector3 currentPosition = transform.position;
        Vector3 currentScale = transform.localScale;

        // y ���� ���̸� �����ϰ� �ٸ� ���� ������ŵ�ϴ�.
        currentScale.y *= Mathf.Pow(stretchFactor, knifeSocre.Value);
        currentScale.x = 1.0f; // x ���� ����
        currentScale.z = 1.0f; // z ���� ����

        // ���̰� ������ ���� �ʵ��� ����
        currentScale.y = Mathf.Max(0.1f, currentScale.y);

        // �������� ����� y �� ���̸�ŭ ��ġ�� �����Ͽ� ������Ʈ�� �߾ӿ� ����
        currentPosition.y += Mathf.Clamp((currentScale.y - transform.localScale.y) * 0.5f, 1, 50);

        // ���ο� ��ġ�� �������� �����մϴ�.
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
