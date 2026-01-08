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

        var keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        if ( Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SpawnProjectile();
        }
    }

    // We are using a ServerRpc here because the Server needs to do all network object spawning.
    [ServerRpc]
    private void SpawnProjectile()
    {
        NetworkObject projectile = Instantiate(ProjectilePrefab, transform.position, Quaternion.identity);
        projectile.transform.up = transform.up; // Orient the projectile to match the gun's direction.

        Spawn(projectile); // NetworkBehaviour shortcut for ServerManager.Spawn(obj);
    }
}
