using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private float _movementSpeed;

    private Vector2 _movementInput;
    private Rigidbody2D _rigidbody2D;
    private PlayerAnimation _playerAnimation;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _playerAnimation = transform.Find("Visual").GetComponent<PlayerAnimation>();
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

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        _rigidbody2D.velocity = _movementInput * _movementSpeed;
    }
    
}
