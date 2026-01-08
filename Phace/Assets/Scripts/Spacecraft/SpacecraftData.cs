using UnityEngine;
using FishNet.Object;
/// <summary>
/// Configuration data for spacecraft. ID, stats, abilities, etc.
/// </summary>

[CreateAssetMenu(fileName = "NewSpacecraftData", menuName = "Spacecraft/Spacecraft Data")]
public class SpacecraftData : ScriptableObject
{
    [SerializeField] private int _spacecraftID;
    [SerializeField] private string _spacecraftName;
    [TextArea][SerializeField] private string _spacecraftDescription;
    [SerializeField] private float _baseMaxSpeed;
    [SerializeField] private float _baseHealth;
    [SerializeField] private float _baseHealthRegen;
    [SerializeField] private float _baseHealthRegenDelay;
    [SerializeField] private float _baseShield;
    [SerializeField] private float _baseShieldRegen;
    [SerializeField] private float _baseShieldRegenDelay;
    [SerializeField] private NetworkObject _spacecraftPrefab;
    [SerializeField] private AttackPatternData _primaryAttackPattern;
    [SerializeField] private AttackPatternData _secondaryAttackPattern;

    public int SpacecraftID => _spacecraftID;
    public string SpacecraftName => _spacecraftName;
    public float BaseMaxSpeed => _baseMaxSpeed;
    public float BaseHealth => _baseHealth;
    public float BaseHealthRegen => _baseHealthRegen;
    public float BaseHealthRegenDelay => _baseHealthRegenDelay;
    public float BaseShield => _baseShield;
    public float BaseShieldRegen => _baseShieldRegen;
    public float BaseShieldRegenDelay => _baseShieldRegenDelay;
    public NetworkObject SpacecraftPrefab => _spacecraftPrefab;
    public AttackPatternData PrimaryAttackPattern => _primaryAttackPattern;
    public AttackPatternData SecondaryAttackPattern => _secondaryAttackPattern;

}