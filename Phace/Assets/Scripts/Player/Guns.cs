using FishNet.Managing.Timing;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using FishNet.Connection;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.VisualScripting; // <- New Input System


public class Guns : NetworkBehaviour
{
    public NetworkObject ProjectilePrefab;

    private void Update()
    {
        // Nur der lokale Spieler darf diese Aktionen ausführen.
        if (!IsOwner)
            return;

        if ( Mouse.current.leftButton.wasPressedThisFrame)
        {
            SpawnProjectile();
        }
    }

    
    [ServerRpc]
    private void SpawnProjectile()
    {
        NetworkObject projectile = Instantiate(ProjectilePrefab, transform.position, Quaternion.identity);
        projectile.transform.up = transform.up; // Orient the projectile to match the gun's direction.

        Spawn(projectile); // NetworkBehaviour shortcut for ServerManager.Spawn(obj);
        if (projectile != null)
        {
            Destroy(projectile.gameObject, 2f);
        }
    }
}
