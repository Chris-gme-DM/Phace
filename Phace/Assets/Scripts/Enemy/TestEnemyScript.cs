using UnityEngine;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine.AI;
using System;
using FishNet.Object.Synchronizing;

public class TestEnemyScript : NetworkBehaviour
{
    
    private NavMeshAgent _agent;
    private int positionIndex;
    [SerializeField] private List<Transform> patrolPoints;
    [SerializeField] private float detectionRadius = 30f;
    [SerializeField] private LayerMask playerLayer;
    private readonly List<Collider2D> results = new List<Collider2D>(16);
    private ContactFilter2D playerFilter;
    private Transform playerInRange;
    public Transform CurrentTarget { get; private set; }
    // network synchronized rotation
    //[SerializeField] private float SyncedRotationZ;
    private readonly SyncVar<float> _syncedRotationZ = new SyncVar<float>();
    public float RotationZ => _syncedRotationZ.Value;

    public override void OnStartClient()
    {
        if (IsServerStarted) return;

        Destroy(GetComponent<NavMeshAgent>());
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        _agent = GetComponent<NavMeshAgent>();
        positionIndex = 0;
        _agent.SetDestination(patrolPoints[positionIndex].position);

        // Configure filter once on server
        playerFilter = new ContactFilter2D();
        playerFilter.SetLayerMask(playerLayer);
        playerFilter.useTriggers = true;

        // Run detection on a timer (NOT every frame)
        InvokeRepeating(nameof(UpdateTarget), 0f, 0.25f);
        

    }


    public override void OnStopServer()
    {
        base.OnStopServer();
        CancelInvoke(nameof(UpdateTarget));
    }

    void Update()
    {
        if (IsServerStarted)
        {
            
            if (playerInRange == null)
            {
                if (!_agent.pathPending && _agent.remainingDistance <= 0.2f)
                {
                    NextPosition();
                }
            }
            else
            {
                Chaseplayer();
            }
            // Server: calculate and send rotation
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

    [Server]
    private int DetectPlayers()
    {
        results.Clear();

        return Physics2D.OverlapCircle(
            transform.position,
            detectionRadius,
            playerFilter,
            results
        );
    }


    [Server]
    private Transform FindClosestPlayer()
    {
        int count = DetectPlayers();

        Transform closest = null;
        float closestDistSqr = float.MaxValue;
        Vector2 origin = transform.position;

        for (int i = 0; i < count; i++)
        {
            Transform player = results[i].transform;

            float distSqr = ((Vector2)player.position - origin).sqrMagnitude;

            if (distSqr < closestDistSqr)
            {
                closestDistSqr = distSqr;
                closest = player;
            }
            Debug.Log(closest);
        }

        playerInRange = closest;
        return closest;
       
    }

    [Server]
    private void UpdateTarget()
    {
        CurrentTarget = FindClosestPlayer();
    }

    // Optional editor visualization (client + server safe)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    [Server]
    private void Chaseplayer()
    {
        if (playerInRange != null)
        {
            _agent.SetDestination(playerInRange.position);
        }
    }


    [Server]
    private void NextPosition()
    {
        positionIndex = (positionIndex + 1) % patrolPoints.Count;
        _agent.SetDestination(patrolPoints[positionIndex].position);
    }

    // synchronize rotation for all clients
    //[ObserversRpc(BufferLast = true)]
    //private void SetRotationObserversRpc(Quaternion rot)
    //{
    //    syncedRotation = rot;
    //}
    [Server]
    private void UpdateRotation()
    {
        if (_agent.velocity.sqrMagnitude > 0.01f)
        {
            Vector3 dir = _agent.velocity.normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            //transform.rotation = Quaternion.Euler(0f, 0f, angle);
            transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);
            transform.Rotate(0, 0, +90f);
            var rot = transform.eulerAngles;
            // Set the value of the SyncVar, not the SyncVar itself
            _syncedRotationZ.Value = rot.z + 90;
            transform.rotation = Quaternion.Euler(rot);
            transform.rotation = Quaternion.Euler(0f, 0f, RotationZ);

        }          
       
        //rot.x = 0;
        //rot.y = 0;
        //transform.rotation = Quaternion.Euler(rot);
    }

}

