using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TestShipProjectile : NetworkBehaviour
{
    public readonly SyncVar<float> syncSpeed = new SyncVar<float>();
    private Rigidbody2D rb;
    private float speed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        speed = 10f; // Set projectile speed
    }

    private void Start()
    {
        TimeManager.OnTick += OnTick;
    }
    private void OnTick()
    {
        float delta = (float)TimeManager.TickDelta;

        Vector2 nextPos = rb.position + (Vector2)(transform.up * speed * delta);

        rb.MovePosition(nextPos);
    }
}
