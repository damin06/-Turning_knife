using Cinemachine;
using System;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private TextMeshPro _nameText;
    [SerializeField] private CinemachineVirtualCamera _followCam;
    [SerializeField] private SpriteRenderer _eyeRenderer;

    public static event Action<Player> OnPlayerSpawned;
    public static event Action<Player> OnPlayerDeSpawned;


    public Health HealthCompo { get; private set; }
    private NetworkVariable<FixedString32Bytes> _username = new NetworkVariable<FixedString32Bytes>();

    private void Awake()
    {
        HealthCompo = GetComponent<Health>();
        HealthCompo.OnDie += HandleDie;
    }

    private void HandleDie(Health health)
    {
        Destroy(gameObject); //여기다가 파티클이나 뭐 죽는 효과 같은게 나와야겠지만...일단은.
    }

    public override void OnNetworkSpawn()
    {
        _username.OnValueChanged += HandleNameChanged;
        HandleNameChanged("", _username.Value);
        if(IsOwner)
        {
            _followCam.Priority = 15;
        }

        if(IsServer)
        {
            OnPlayerSpawned?.Invoke(this);
        }
    }

    public override void OnNetworkDespawn()
    {
        _username.OnValueChanged -= HandleNameChanged;
        if(IsServer)
        {
            OnPlayerDeSpawned?.Invoke(this);
        }
    }

    public FixedString32Bytes GetUserName()
    {
        return _username.Value;
    }

    private void HandleNameChanged(FixedString32Bytes prev, FixedString32Bytes newValue)
    {
        _nameText.text = newValue.ToString();
    }

    public void SetUserName(string username)
    {
        _username.Value = username;
    }

    public void SetEyeColor(Color eyeColor)
    {
        _eyeRenderer.color = eyeColor;
    }
}
