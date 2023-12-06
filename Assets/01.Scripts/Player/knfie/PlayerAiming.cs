using System.Runtime.InteropServices;
using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    public NetworkVariable<int> _curDir = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private Transform _handTrm;
    [SerializeField] private float _rotateSpeed = 3f;
    private float curZRoate;
    private PlayerAnimation _playerAnimation;

    public override void OnNetworkSpawn()
    {
       
    }

    private void Awake()
    {
        _playerAnimation = GetComponent<PlayerAnimation>();   
    }

    public void ChangeDir()
    {
        if (!IsServer)
            return;
        _playerAnimation.AttackAnimationServerRpc();
        _curDir.Value *= -1;
    }
    
    private void Update()
    {
        if (!IsOwner) return;

        curZRoate += Time.deltaTime;

        //_handTrm.transform.right = new Vector3(0, 0, transform.rotation.z + Time.deltaTime);
        _handTrm.transform.rotation = Quaternion.Euler(0, 0, _rotateSpeed * curZRoate * _curDir.Value);
        if (curZRoate * _rotateSpeed >= 360)
            curZRoate = 0;
    }
}
