using UnityEngine;
using System;
using FishNet.Object;
/// <summary>
/// Configuration data for spacecraft. ID, stats, abilities, etc.
/// </summary>

[CreateAssetMenu(fileName = "NewSpacecraftData", menuName = "Spacecraft/Spacecraft Data")]
public class SpacecraftData : ScriptableObject
{
    [Header("Spacecraft Configuration")]
    [SerializeField] private int _spacecraftID;
    [SerializeField] private string _spacecraftName;
    [TextArea][SerializeField] private string _spacecraftDescription;
    [SerializeField] private AssociationType _association;
    //[SerializeField] private float _spacecraftSize;
    [SerializeField] private GameObject _goSpacecraftPrefab;
    [SerializeField] private AttackPattern _primaryAttackPattern;
    [SerializeField] private AttackPattern _secondaryAttackPattern;
    [Header("Base Stats")]
    [SerializeField] private float _baseMaxSpeed;
    [SerializeField] private float _baseHealth;
    [SerializeField] private float _baseHealthRegen;
    [SerializeField] private float _baseHealthRegenDelay;
    [SerializeField] private float _baseShield;
    [SerializeField] private float _baseShieldRegen;
    [SerializeField] private float _baseShieldRegenDelay;

    public int SpacecraftID => _spacecraftID;
    public string SpacecraftName => _spacecraftName;
    //public float SpacecraftSize => _spacecraftSize;
    public float BaseMaxSpeed => _baseMaxSpeed;
    public float BaseHealth => _baseHealth;
    public float BaseHealthRegen => _baseHealthRegen;
    public float BaseHealthRegenDelay => _baseHealthRegenDelay;
    public float BaseShield => _baseShield;
    public float BaseShieldRegen => _baseShieldRegen;
    public float BaseShieldRegenDelay => _baseShieldRegenDelay;
    public GameObject GOSpacecraftPrefab => _goSpacecraftPrefab;
    public Sprite Icon => _goSpacecraftPrefab.GetComponentInChildren<SpriteRenderer>().sprite;
    public AssociationType Association => _association;
    public AttackPattern PrimaryAttackPattern => _primaryAttackPattern;
    public AttackPattern SecondaryAttackPattern => _secondaryAttackPattern;
}
/// <summary>
/// Holds current stats for a spacecraft instance.
/// </summary>
/// 
[Serializable]
public struct SpacecraftStats
{
    // Spacecraft stats
    public float CurrentHealth;
    public float MaxHealth;
    public float HealthRegenRate;
    public float HealthRegenDelay;
    public float CurrentShield;
    public float MaxShield;
    public float ShieldRegenRate;
    public float ShieldRegenDelay;
    public float MoveSpeed;
    public float MaxSpeed;

    // Other Stuff
    public AssociationType Association;
    public float Size;
}