using FishNet.Managing.Timing;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using FishNet.Connection;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using FishNet.Demo.HashGrid; // <- New Input System


public class Guns : NetworkBehaviour
{
    //public NetworkObject ProjectilePrefab;
    private ProjectileSpawnManager bulletSpawner;

    private void Start()
    {
        
        bulletSpawner = FindAnyObjectByType<ProjectileSpawnManager>();
    }

    private void Update()
    {
        // Nur der lokale Spieler darf diese Aktionen ausführen.
        if (!IsOwner)
            return;

        if ( Mouse.current.leftButton.wasPressedThisFrame)
        {

            GunsSpawnSingleProjectile();
        }
    
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            GunsSpawnSpreadShot();
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            GunsSpawnHomingShot();
        }


    }


    //[ServerRpc]
    //private void SpawnProjectile()
    //{
    //    NetworkObject projectile = Instantiate(ProjectilePrefab, transform.position, Quaternion.identity);
    //    projectile.transform.up = transform.up; // Orient the projectile to match the gun's direction.

    //    Spawn(projectile); // NetworkBehaviour shortcut for ServerManager.Spawn(obj);
    //    if (projectile != null)
    //    {
    //        Destroy(projectile.gameObject, 2f);
    //    }
    //}


    [ServerRpc]
    private void GunsSpawnSingleProjectile()
    {
        Debug.Log(bulletSpawner == null);
        bulletSpawner.SpawnSingleProjectile(transform.position, transform.up);
    }

    [ServerRpc]
    private void GunsSpawnSpreadShot()
    {
        // Basisrichtung
        var baseDirection = transform.up;

        // Streuwinkel in Grad
        float spreadAngle = 15f;

        // Links rotieren
        var spreadLeft = Quaternion.Euler(0, 0, -spreadAngle) * baseDirection;
        // Rechts rotieren
        var spreadRight = Quaternion.Euler(0, 0, spreadAngle) * baseDirection;

        bulletSpawner.SpawnSpreadShot(transform.position, baseDirection, spreadLeft, spreadRight);
    }

    [ServerRpc]
    private void GunsSpawnHomingShot()
    {
        bulletSpawner.SpawnHomingShot(transform.position, transform.up);
    }



}
