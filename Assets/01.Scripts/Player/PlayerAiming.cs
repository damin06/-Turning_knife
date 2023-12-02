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
    //���⼱ ���콺 ��ġ�� _inputReader AimPosition�� �޾Ƽ� 
    //�����ϰ� handTrm�� ȸ�������ָ� �ȴ�.
    private void LateUpdate()
    {
        if (!IsOwner) return;
        //float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //_handTrm.rotation = Quaternion.Euler(0, 0, angle);
        _handTrm.transform.rotation = new Quaternion(0, 0, transform.rotation.z + Time.deltaTime * _curDir, 0);
        //_handTrm.right = dir;
    }
}
