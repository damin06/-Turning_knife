using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private float _movementSpeed;

    [Header("EYE")]
    [SerializeField][Min(0.1f)] private float _eyeMoveSpeed = 2f;
    [SerializeField] private float _eyeMaxDist = 0.2f;
    [SerializeField] private Transform _pupilPos;
    [SerializeField] private Transform _irisPos;

    private Vector2 _movementInput;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;

    private readonly string Eye_Attack = "Attack";
    private readonly string Eye_Hurt = "Hurt";

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        _inputReader.MovementEvent += HandleMovement;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        _inputReader.MovementEvent -= HandleMovement;
    }

    private void HandleMovement(Vector2 movementInput)
    {
        _movementInput = movementInput;
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        _irisPos.transform.localPosition = Vector3.Lerp(_irisPos.transform.localPosition, _movementInput * _eyeMaxDist, Time.deltaTime * _eyeMoveSpeed);
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        _rigidbody2D.velocity = _movementInput * _movementSpeed;
    }

    [ClientRpc]
    public void AttackAnimationClientRpc() 
    {
        _animator.SetTrigger(Eye_Attack);
    } 

    [ClientRpc]
    public void HurtAnimationClientRpc()
    {
        _animator.SetTrigger(Eye_Hurt);
    }
    
}
