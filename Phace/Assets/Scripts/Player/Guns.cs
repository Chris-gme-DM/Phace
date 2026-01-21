using FishNet.Managing.Timing;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;


public class Guns : NetworkBehaviour
{
    //public NetworkObject ProjectilePrefab;
    private ProjectileSpawnManager bulletSpawner;
    private float shootIntervalSingleShot = 0f;
    private float shootIntervalSpreadShot = 0f;
    private float shootIntervalHomingShot = 0f;

    [SerializeField] private float shootDelaySingleShot = 0.5f;
    [SerializeField] private float shootDelaySpreadShot = 1.5f;
    [SerializeField] private float shootDelayHomingShot = 4f;

    private bool canShootSingle = true;
    private bool canShootSpread = true;
    private bool canShootHoming = true;



    public override void OnStartServer()
    {
        base.OnStartServer();
        TimeManager.OnTick += OnTick;

    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        TimeManager.OnTick -= OnTick;
    }
    private void Start()
    {
        
        bulletSpawner = FindAnyObjectByType<ProjectileSpawnManager>();
    }

    //private void Update()
    //{
    //    // Nur der lokale Spieler darf diese Aktionen ausführen.
    //    if (!IsOwner)
    //        return;

    //    if ( Mouse.current.leftButton.wasPressedThisFrame)
    //    {

    //        GunsSpawnSingleProjectile();
    //    }

    //    if (Mouse.current.rightButton.wasPressedThisFrame)
    //    {
    //        GunsSpawnSpreadShot();
    //    }

    //    if (Keyboard.current.spaceKey.wasPressedThisFrame)
    //    {
    //        GunsSpawnHomingShot();
    //    }


    //}
    private void OnTick()
    {
        if (!IsServerInitialized)
            return;
        float tickDelta = (float)TimeManager.TickDelta;
        if (shootIntervalSingleShot > 0f)
        {
            shootIntervalSingleShot -= tickDelta;
            canShootSingle = false;

        }
        else
        {
            canShootSingle = true;
        }

        if (shootIntervalSpreadShot > 0f)
        {
            shootIntervalSpreadShot -= tickDelta;
            canShootSpread = false;
        }
        else
        {
            canShootSpread = true;
        }

        if (shootIntervalHomingShot > 0f)
        {
            shootIntervalHomingShot -= tickDelta;
            canShootHoming = false;
        }
        else
        {
            canShootHoming = true;
        }
    }
    public void OnAttackPrimary(InputAction.CallbackContext ctx) 
    { 
        if(!IsOwner || !canShootSingle) return;    
        GunsSpawnSingleProjectile();
        shootIntervalSingleShot = shootDelaySingleShot;
    }

    public void OnAttackSecondary(InputAction.CallbackContext ctx) 
    { 
        if(!IsOwner || !canShootSpread) return;    
        GunsSpawnSpreadShot();
        shootIntervalSpreadShot = shootDelaySpreadShot;
    }

    public void OnHomingMissle(InputAction.CallbackContext ctx) 
    { 
        if(!IsOwner || !canShootHoming) return;    
        GunsSpawnHomingShot();
        shootIntervalHomingShot = shootDelayHomingShot;
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
