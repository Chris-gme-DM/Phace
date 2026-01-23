using UnityEngine;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine.AI;
using System;
using FishNet.Object.Synchronizing;

public class EnemyTypeC : NetworkBehaviour, IDamageable
{

    private NavMeshAgent _agent;
    private int positionIndex;
    private List<Transform> patrolPoints;
    [SerializeField] private float detectionRadius = 30f;
    [SerializeField] private LayerMask playerLayer;
    public Transform CurrentTarget { get; private set; }
    private readonly SyncVar<float> _syncedRotationZ = new SyncVar<float>();
    public float RotationZ => _syncedRotationZ.Value;
    private EnemySpawnManager spawner;
    private ProjectileSpawnManager bulletSpawner;
    private float shootInterval = 0f;
    [SerializeField] private float shootDelay = 2f;


    [SerializeField] private int health = 1;


    public override void OnStartServer()
    {
        base.OnStartServer();
        _agent = GetComponent<NavMeshAgent>();
        spawner = FindAnyObjectByType<EnemySpawnManager>();
        bulletSpawner = FindAnyObjectByType<ProjectileSpawnManager>();
        patrolPoints = spawner.patrolPointsC;

        positionIndex = 0;
        _agent.SetDestination(patrolPoints[positionIndex].position);
        TimeManager.OnTick += OnTick;
    }

    public override void OnStartClient()
    {
        if (IsServerStarted) return;

        Destroy(GetComponent<NavMeshAgent>());
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        //CancelInvoke(nameof(UpdateTarget));
        TimeManager.OnTick -= OnTick;
        spawner?.NotifyEnemyDestroyed(GetComponent<NetworkObject>());
    }


    void Update()
    {
        if (IsServerStarted)
        {
            if (!_agent.pathPending && _agent.remainingDistance <= 0.2f)
            {
                NextPosition();
            }

            UpdateRotation();
        }
    }


    private void LateUpdate()
    {
        if (IsServerStarted || IsClientStarted)
        {
            // Client: apply synchronized rotation
            transform.rotation = Quaternion.Euler(0f, 0f, RotationZ);
        }
    }
    private void OnTick()
    {
        if (!IsServerInitialized)
            return;
        float tickDelta = (float)TimeManager.TickDelta;
        if (shootInterval > 0f)
        {
            shootInterval -= tickDelta;
        }
        else if (shootInterval <= 0f)
        {
            shootAtPlayer();
            shootInterval += shootDelay;
        }
    }


    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }


    [Server]
    private void NextPosition()
    {
        positionIndex = (positionIndex + 1) % patrolPoints.Count;
        _agent.SetDestination(patrolPoints[positionIndex].position);
    }


    [Server]
    private void UpdateRotation()
    {

        if (_agent.velocity.sqrMagnitude > 0.01f)
        {
            Vector2 dir = _agent.velocity.normalized;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            angle -= 90f; // Sprite correction

            _syncedRotationZ.Value = angle;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

    }

    [Server]
    private void shootAtPlayer()
    {
        if  (bulletSpawner != null)
        {
            bulletSpawner.Enemy4WayShot(transform.position);

        }
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
