using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
/// <summary>
/// This class represents a spacecraft in the game, managing its stats and actions.
/// </summary>
public class Spacecraft : NetworkBehaviour
{
    #region Data/Networking
    public SpacecraftData SpacecraftData;
    public readonly SyncVar<SpacecraftStats> Stats = new();
    public readonly SyncVar<Transform> SpawnInformation = new();
    public readonly SyncVar<AssociationType> Association = new();

    public override void OnStartServer()
    {
        base.OnStartServer();


    }
    public void Initialize(SpacecraftData data)
    {
        SpacecraftData = data;
        var s = new SpacecraftStats();
        if (data != null)
        {
            s.CurrentHealth = SpacecraftData.BaseHealth,
            s.MaxHealth = SpacecraftData.BaseHealth,
            s.HealthRegenRate = SpacecraftData.BaseHealthRegen,
            s.HealthRegenDelay = SpacecraftData.BaseHealthRegenDelay,
            s.CurrentShield = SpacecraftData.BaseShield,
            s.MaxShield = SpacecraftData.BaseShield,
            s.ShieldRegenRate = SpacecraftData.BaseShieldRegen,
            s.ShieldRegenDelay = SpacecraftData.BaseShieldRegenDelay,
            s.MoveSpeed = 0f;
            s.MaxSpeed = SpacecraftData.BaseMaxSpeed,
            
            s.Association = SpacecraftData.Association,

        }
    }
    #endregion
    #region Handlers
    [ServerRpc]
    public void TakeDamage(float amount)
    {

    }
    [ServerRpc]
    public void Repair(float amount)
    {

    }
    [ServerRpc]
    public void RechargeShield(float amount)
    {

    }
    [ServerRpc]
    public void HandleMovement(Vector2 direction, float speed) { }

    [ServerRpc]
    public void HandlePrimaryAttack(int attackPatternID) { }

    [ServerRpc]
    public void HandleSecondaryAttack(int attackPatternID) { }
}
#endregion
