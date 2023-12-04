using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private Transform _handTrm;
    [SerializeField] private float _rotateSpeed = 3f;
    private float curZRoate;
    public int _curDir = 1;

    public override void OnNetworkSpawn()
    {
       
    }

    private void Start()
    {
    }

    public void ChangeDir() => _curDir *= -1;
    
    private void Update()
    {
        if (!IsOwner) return;

        curZRoate += Time.deltaTime;

        //_handTrm.transform.right = new Vector3(0, 0, transform.rotation.z + Time.deltaTime);
        _handTrm.transform.rotation = Quaternion.Euler(0, 0, _rotateSpeed * curZRoate * _curDir);
        if (curZRoate * _rotateSpeed >= 360)
            curZRoate = 0;
    }
}
