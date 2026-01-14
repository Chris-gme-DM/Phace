using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestShiptHoming : NetworkBehaviour
{
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private LayerMask enemyLayer;
     [SerializeField] private float speed;
    private readonly List<Collider2D> results = new List<Collider2D>(5);
    private ContactFilter2D enemyFilter;
    private Transform enemyInRange;
    public Transform CurrentTarget { get; private set; }
    public readonly SyncVar<float> syncSpeed = new SyncVar<float>();
    private Rigidbody2D rb;

    public override void OnStartServer()
    {
        base.OnStartServer();


        // Configure filter once on server
        enemyFilter = new ContactFilter2D();
        enemyFilter.SetLayerMask(enemyLayer);
        enemyFilter.useTriggers = true;

        // Run detection on a timer (NOT every frame)
        InvokeRepeating(nameof(UpdateTarget), 0f, 0.25f);


    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        CancelInvoke(nameof(UpdateTarget));
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    private void Start()
    {
        TimeManager.OnTick += OnTick;
    }

    [Server]
    private void UpdateTarget()
    {
        CurrentTarget = FindClosestEnemy();
    }

    [Server]
    private Transform FindClosestEnemy()
    {
        int count = DetectEnemies();

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

        enemyInRange = closest;
        return closest;

    }

    [Server]
    private int DetectEnemies()
    {
        results.Clear();

        return Physics2D.OverlapCircle(
            transform.position,
            detectionRadius,
            enemyFilter,
            results
        );
    }

    private void OnTick()
    {
        if (!IsServerStarted)
            return;
        if (rb != null)
        {
            if (enemyInRange != null)
            {
                transform.up = (enemyInRange.position - transform.position).normalized;
                float delta = (float)TimeManager.TickDelta;

                Vector2 nextPos = rb.position + (Vector2)(transform.up * speed * delta);

                rb.MovePosition(nextPos);

            }
            else
            {
                float delta = (float)TimeManager.TickDelta;
                Vector2 nextPos = rb.position + (Vector2)(transform.up * speed * delta);
                rb.MovePosition(nextPos);
            }
        }
    }

}
