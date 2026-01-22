using FishNet.Connection;
using FishNet.Managing.Timing;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using GameKit.Dependencies.Utilities.ObjectPooling.Examples;
using System.Collections;
using UnityEngine;

public class ProjectileSpawnManager : NetworkBehaviour
{
    public NetworkObject ProjectilePrefab;
    public NetworkObject HomingProjectilePrefab;
    public NetworkObject EnemyProjectileTypeAPrefab;
    public NetworkObject EnemyProjectileTypeBPrefab;
    [SerializeField] private float lifetimeSecondsProjectile = 3f;
    [SerializeField] private float lifetimeSecondsHomingProjectile = 5f;
    [Server]
    public void SpawnSingleProjectile(Vector3 position, Vector3 direction)
    {
        NetworkObject projectile = Instantiate(ProjectilePrefab, position, Quaternion.identity);
        projectile.transform.up = direction;
        Spawn(projectile); // NetworkBehaviour shortcut for ServerManager.Spawn(obj);
        if (projectile != null)
        {
            
            StartCoroutine(DespawnAfterTime(projectile, lifetimeSecondsProjectile));
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
            StartCoroutine(DespawnAfterTime(spreadProjectile1, lifetimeSecondsProjectile));
        }

        NetworkObject spreadProjectile2 = Instantiate(ProjectilePrefab, position, Quaternion.identity);
        spreadProjectile2.transform.up = direction;
        Spawn(spreadProjectile2); // NetworkBehaviour shortcut for ServerManager.Spawn(obj);
        if (spreadProjectile2 != null)
        {
            StartCoroutine(DespawnAfterTime(spreadProjectile2, lifetimeSecondsProjectile));
        }

        NetworkObject spreadProjectile3 = Instantiate(ProjectilePrefab, position, Quaternion.identity);
        spreadProjectile3.transform.up = directionRight;
        Spawn(spreadProjectile3); // NetworkBehaviour shortcut for ServerManager.Spawn(obj);
        if (spreadProjectile3 != null)
        {
            StartCoroutine(DespawnAfterTime(spreadProjectile3, lifetimeSecondsProjectile));
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
            StartCoroutine(DespawnAfterTime(homingProjectile, lifetimeSecondsHomingProjectile)); 
        }
    }
    
    
    //Gegnerische Projektile//---------------------------------------------------------

    [Server]
    public void SpawnEnemyProjectileTypeA(Vector3 position, Vector3 direction)
        {
        NetworkObject enemyProjectile = Instantiate(EnemyProjectileTypeAPrefab, position, Quaternion.identity);
        enemyProjectile.transform.up = direction; // Orient the projectile to match the gun's direction.
        Spawn(enemyProjectile); // NetworkBehaviour shortcut for ServerManager.Spawn(obj);
        if (enemyProjectile != null)
        {
            StartCoroutine(DespawnAfterTime(enemyProjectile, lifetimeSecondsProjectile));
        }
    }

    [Server]
    public void SpawnEnemySpreadShot(Vector3 position, Vector3 direction, Vector3 directionLeft, Vector3 directionRight)
    {
         NetworkObject enemySpreadProjectile1 = Instantiate(EnemyProjectileTypeBPrefab, position, Quaternion.identity);
        enemySpreadProjectile1.transform.up = directionLeft;
        Spawn(enemySpreadProjectile1); // NetworkBehaviour shortcut for ServerManager.Spawn(obj);
        if (enemySpreadProjectile1 != null)
        {
            StartCoroutine(DespawnAfterTime(enemySpreadProjectile1, lifetimeSecondsProjectile));
        }
        NetworkObject enemySpreadProjectile2 = Instantiate(EnemyProjectileTypeBPrefab, position, Quaternion.identity);
        enemySpreadProjectile2.transform.up = direction;
        Spawn(enemySpreadProjectile2); // NetworkBehaviour shortcut for ServerManager.Spawn(obj);
        if (enemySpreadProjectile2 != null)
        {
            StartCoroutine(DespawnAfterTime(enemySpreadProjectile2, lifetimeSecondsProjectile));
        }
        NetworkObject enemySpreadProjectile3 = Instantiate(EnemyProjectileTypeBPrefab, position, Quaternion.identity);
        enemySpreadProjectile3.transform.up = directionRight;
        Spawn(enemySpreadProjectile3); // NetworkBehaviour shortcut for ServerManager.Spawn(obj);
        if (enemySpreadProjectile3 != null)
        {
            StartCoroutine(DespawnAfterTime(enemySpreadProjectile3, lifetimeSecondsProjectile));
        }
    }

    [Server]
    public void Enemy4WayShot(Vector3 position)
    {
        float angleStep = 90f;
               
            for (int i = 0; i < 4; i++)
            {
                float angle = i * angleStep;
                Vector3 shotDirection = Quaternion.Euler(0f, 0f, angle) * Vector3.up;
                NetworkObject projectile = Instantiate(EnemyProjectileTypeAPrefab, position, Quaternion.identity);
                projectile.transform.up = shotDirection;
                Spawn(projectile);
                StartCoroutine(DespawnAfterTime(projectile, lifetimeSecondsProjectile));
            }
    }



    [Server]
    public void EnemyBossShot(Vector3 position,int projectileCount)
    {
        float angleStep = 360f / projectileCount;
               
            for (int i = 0; i < projectileCount; i++)
            {
                float angle = i * angleStep;

                Vector3 shotDirection =
                    Quaternion.Euler(0f, 0f, angle) * Vector3.up;

                NetworkObject projectile =
                    Instantiate(EnemyProjectileTypeAPrefab, position, Quaternion.identity);

                projectile.transform.up = shotDirection;

                Spawn(projectile);
                StartCoroutine(DespawnAfterTime(projectile, lifetimeSecondsProjectile));
            }
    }


    

    IEnumerator DespawnAfterTime(NetworkObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        if (obj != null)
        {
            Despawn(obj, DespawnType.Destroy);
        }
    }
}
