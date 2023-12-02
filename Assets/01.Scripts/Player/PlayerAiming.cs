using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private Transform _handTrm;
    private int _curDir;

    public override void OnNetworkSpawn()
    {
        if(!IsServer)
            return;
        _curDir = 1;
    }

    public void ChangeDir() => _curDir *= -1;
    //여기선 마우스 위치를 _inputReader AimPosition을 받아서 
    //적절하게 handTrm을 회전시켜주면 된다.
    private void LateUpdate()
    {
        if (!IsOwner) return;
        //float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //_handTrm.rotation = Quaternion.Euler(0, 0, angle);
        _handTrm.transform.rotation = new Quaternion(0, 0, transform.rotation.z + Time.deltaTime * _curDir, 0);
        //_handTrm.right = dir;
    }
}
