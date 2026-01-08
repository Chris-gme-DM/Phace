using FishNet.Object;
using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectileData", menuName = "AttackPatterns/Projectile Data")]
public class ProjectileData : ScriptableObject
{
    [SerializeField] private NetworkObject _projectilePrefab;
    [SerializeField] private float _speed;
    [SerializeField] private float _damage;
    [SerializeField] private float _lifeTime;
    [SerializeField] private ProjectileType _projectileType;
    [SerializeField] private HitType _hitType;

    public NetworkObject ProjectilePrefab => _projectilePrefab;
    public float Speed => _speed;
    public float Damage => _damage;
    public float LifeTime => _lifeTime;
}

public enum ProjectileType
{
    Bullet,
    Missile,
    Laser,
    Plasma
}
public enum HitType
{
    Instant,
    AreaOfEffect,
    Piercing,
    DamageOverTime
}