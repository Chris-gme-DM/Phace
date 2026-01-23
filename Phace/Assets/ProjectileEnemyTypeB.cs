using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileEnemyTypeB : NetworkBehaviour
{
    public readonly SyncVar<float> syncSpeed = new SyncVar<float>();
    private Rigidbody2D rb;
    [SerializeField] private float speed;
    [SerializeField] private int damage = 10;
    private float jitter = 0f;
    private float jitterstep = 0.8f;
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
        if (rb != null)
        {
            float delta = (float)TimeManager.TickDelta;
            Vector2 dir = transform.up;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            angle -= 90f;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            jitter += jitterstep;
            if (jitter >= 5f || jitter <= -5f)
            {
                jitterstep = -jitterstep;
            }

            jitter = Mathf.Clamp(jitter, -5f, 5f);



            Vector2 nextPos = rb.position + (Vector2)(transform.up * speed * delta) + (Vector2)(transform.right * jitter * delta);
            rb.MovePosition(nextPos);


        }
    }

    [Server]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
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
}