using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackPatternData", menuName = "AttackPatterns/Attack Pattern Data")]
public class AttackPatternData : ScriptableObject
{
    [SerializeField] private int _attackPatternID;
    [SerializeField] private string _attackPatternName;
    [TextArea][SerializeField] private string _attackPatternDescription;
    [Tooltip("Number of projectiles to be fired in this attack pattern")]
    [SerializeField] private int _numberOfProjectiles;

    [SerializeField] private float _fireRate;
    [SerializeField] private float _AttackCooldown;
    [SerializeField] private float spreadAngle;
    [SerializeField] private Projectile _projectile;

    public int AttackPatternID => _attackPatternID;
    public string AttackPatternName => _attackPatternName;
    public int NumberOfProjectiles => _numberOfProjectiles;
    public float FireRate => _fireRate;
    public float AttackCooldown => _AttackCooldown;
    public Projectile Projectile => _projectile;

}