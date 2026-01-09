using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
/// <summary>
/// This class represents a spacecraft in the game, managing its stats and actions.
/// </summary>
public class Spacecraft : NetworkBehaviour
{
    #region Data/Networking
    public readonly SpacecraftData SpacecraftData;
    public readonly SyncVar<SpacecraftStats> Stats = new();

    public override void OnStartServer()
    {
        base.OnStartServer();
        Stats.Value = new SpacecraftStats
        {
            CurrentHealth = SpacecraftData.BaseHealth,
            MaxHealth = SpacecraftData.BaseHealth,
            HealthRegenRate = SpacecraftData.BaseHealthRegen,
            HealthRegenDelay = SpacecraftData.BaseHealthRegenDelay,
            CurrentShield = SpacecraftData.BaseShield,
            MaxShield = SpacecraftData.BaseShield,
            ShieldRegenRate = SpacecraftData.BaseShieldRegen,
            ShieldRegenDelay = SpacecraftData.BaseShieldRegenDelay,
            MoveSpeed = 0f,
            MaxSpeed = SpacecraftData.BaseMaxSpeed,

        };
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
