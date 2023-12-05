using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Coin : NetworkBehaviour
{
    private NetworkVariable<int> _coinSocre = new NetworkVariable<int>(1,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public int GetCoinScoreS()
    {
        return _coinSocre.Value;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer)
            return;

        if (collision.TryGetComponent<Knife>(out Knife knife))
        {
            knife.AddknifeSocre(_coinSocre.Value);
            GetComponent<NetworkObject>().Despawn(true);
            Destroy(gameObject);
        }
    }
}
