using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using System.Collections.Generic;
public class PlayerStatsManager : NetworkBehaviour, IDamageable
{
    [SerializeField] private float health = 3;
    private readonly SyncVar<float> _syncedHealth = new SyncVar<float>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public override void OnStartServer()
    {
        base.OnStartServer();
        _syncedHealth.Value = health;
    }

    [Server]

    private void Update()
    {
        _syncedHealth.Value = health;
    }

    [Server]
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            NetworkObject.Despawn();
        }
    }


}
