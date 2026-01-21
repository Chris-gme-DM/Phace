using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TestShipProjectile : NetworkBehaviour
{
    public readonly SyncVar<float> syncSpeed = new SyncVar<float>();
    private Rigidbody2D rb;
    [SerializeField] private float speed;
    [SerializeField] private int damage = 1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    private void Start()
    {
        TimeManager.OnTick += OnTick;
    }
    private void OnTick()
    {
        if (!IsServerStarted)
            return;
        if (rb != null) { 
            float delta = (float)TimeManager.TickDelta;

            Vector2 nextPos = rb.position + (Vector2)(transform.up * speed * delta);

            rb.MovePosition(nextPos);
        }
    }

    [Server]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            var damageable = collision.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }

            if (NetworkObject != null)
            {
                NetworkObject.Despawn();
            }
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision) 
    //{ 
    //    if (collision.gameObject.CompareTag("Enemy")) 
    //    { 
    //        TestEnemyScript enemy = collision.GetComponentInParent<TestEnemyScript>(); 
    //        if (enemy != null) 
    //        { 
    //            enemy.TakeDamage(damage); 
    //        } 
    //        if (NetworkObject != null) 
    //        { 
    //            NetworkObject.Despawn(); 
    //        } 
    //    } 
    //}
}







