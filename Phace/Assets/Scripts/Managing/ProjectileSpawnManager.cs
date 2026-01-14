using UnityEngine;
using FishNet.Managing.Timing;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using FishNet.Connection;
using FishNet.Object;

public class ProjectileSpawnManager : NetworkBehaviour
{
    public NetworkObject ProjectilePrefab;
    public NetworkObject HomingProjectilePrefab;
    [Server]
    public void SpawnSingleProjectile(Vector3 position, Vector3 direction)
    {
        NetworkObject projectile = Instantiate(ProjectilePrefab, position, Quaternion.identity);
        projectile.transform.up = direction; // Orient the projectile to match the gun's direction.
        Spawn(projectile); // NetworkBehaviour shortcut for ServerManager.Spawn(obj);
        if (projectile != null)
        {
            Destroy(projectile.gameObject, 2f);
        }
    }

    [Server]
    public void SpawnSpreadShot(Vector3 position, Vector3 direction, Vector3 directionLeft, Vector3 directionRight) 
    {
        NetworkObject spreadProjectile1 = Instantiate(ProjectilePrefab, position, Quaternion.identity);
        spreadProjectile1.transform.up = directionLeft; 
        Spawn(spreadProjectile1); // NetworkBehaviour shortcut for ServerManager.Spawn(obj);
        if (spreadProjectile1 != null)
        {
            Destroy(spreadProjectile1.gameObject, 2f);
        }

        NetworkObject spreadProjectile2 = Instantiate(ProjectilePrefab, position, Quaternion.identity);
        spreadProjectile2.transform.up = direction;
        Spawn(spreadProjectile2); // NetworkBehaviour shortcut for ServerManager.Spawn(obj);
        if (spreadProjectile2 != null)
        {
            Destroy(spreadProjectile2.gameObject, 2f);
        }

        NetworkObject spreadProjectile3 = Instantiate(ProjectilePrefab, position, Quaternion.identity);
        spreadProjectile3.transform.up = directionRight;
        Spawn(spreadProjectile3); // NetworkBehaviour shortcut for ServerManager.Spawn(obj);
        if (spreadProjectile3 != null)
        {
            Destroy(spreadProjectile3.gameObject, 2f);
        }

    }

    [Server]
    public void SpawnHomingShot(Vector3 position, Vector3 direction)
    {
        NetworkObject homingProjectile = Instantiate(HomingProjectilePrefab, position, Quaternion.identity);
        homingProjectile.transform.up = direction; // Orient the projectile to match the gun's direction.
        Spawn(homingProjectile); // NetworkBehaviour shortcut for ServerManager.Spawn(obj);
        if (homingProjectile != null)
        {
            Destroy(homingProjectile.gameObject, 5f);
        }
    }


}
