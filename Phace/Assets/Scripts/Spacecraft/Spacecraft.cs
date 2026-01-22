using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet;
/// <summary>
/// This class represents a spacecraft in the game, managing its stats and actions.
/// </summary>
public class Spacecraft : NetworkBehaviour, IDamageable
{
    #region Data/Networking
    public SpacecraftData SpacecraftData;
    public readonly SyncVar<SpacecraftStats> Stats = new();
    public readonly SyncVar<AssociationType> Association = new();

    private float _lastHit;
    public override void OnStartServer()
    {
        base.OnStartServer();
        Stats.OnChange += HandleStatChanged;
    }
    public void Initialize(SpacecraftData data)
    {
        SpacecraftData = data;
        var s = new SpacecraftStats();
        if (data != null)
        {
            s.CurrentHealth = data.BaseHealth;
            s.MaxHealth = data.BaseHealth;
            s.HealthRegenRate = data.BaseHealthRegen;
            s.HealthRegenDelay = data.BaseHealthRegenDelay;
            s.CurrentShield = data.BaseShield;
            s.MaxShield = data.BaseShield;
            s.ShieldRegenRate = data.BaseShieldRegen;
            s.ShieldRegenDelay = data.BaseShieldRegenDelay;
            //s.MoveSpeed = 0f;
            s.MaxSpeed = data.BaseMaxSpeed;

            s.Association = SpacecraftData.Association;
        }
        Stats.Value = s;
        InstanceFinder.TimeManager.OnTick += OnTick;
    }
    [Server]
    private void OnTick()
    {
        float timeLastHit = Time.time - _lastHit;
        if (timeLastHit >= Stats.Value.ShieldRegenDelay)
        {
            RechargeShield(0);
        }
        if (timeLastHit >= Stats.Value.HealthRegenDelay)
        {
            Repair(0);
        }
    }
    #endregion
    #region Handlers
    /// <summary>
    /// Most mehtods that handle changes to the in game stats of a spacecraft. If we want to access some of these methods from outside, they have to be changed to public
    /// Repair and RechargeShield will need some tweeking before launch, for balance reasons
    /// </summary>
    /// <param name="amount"></param>
    [Server]
    private void Repair(int amount)
    {
        SpacecraftStats stat = Stats.Value;
        stat.CurrentHealth += amount;
        stat.CurrentHealth += stat.HealthRegenRate/100; // adjustment for the feel of a regen rate and the actual gameplay tick
        stat.CurrentHealth = Mathf.Max(stat.CurrentHealth, stat.MaxHealth);
    }
    [Server]
    private void RechargeShield(int amount)
    {
        SpacecraftStats stats = Stats.Value;
        stats.CurrentShield += amount;
        stats.CurrentShield += stats.ShieldRegenRate/100;  // adjustment for the feel of a regen rate and the actual gameplay tick
        stats.CurrentShield = Mathf.Max(stats.CurrentShield, stats.MaxShield);
    }
    [Server]
    public void TakeDamage(int amount)
    {
        _lastHit = Time.time;
        SpacecraftStats stat = Stats.Value;
        stat.CurrentHealth -= amount;
        stat.CurrentHealth = Mathf.Max(0, stat.CurrentHealth);
        if (stat.CurrentHealth <= 0)
        {
            InstanceFinder.TimeManager.OnTick -= OnTick;
            if (stat.Association == AssociationType.Player)
            {
                GameEvents.OnPlayerDestroyed.Invoke();
            }
            else
            {
                GameEvents.OnEnemyDestroyed.Invoke();
            }
            Despawn();
        }
    }
    private void HandleStatChanged(SpacecraftStats prev, SpacecraftStats next, bool asServer)
    {
        if (asServer) return;
        if (next.Association == AssociationType.Player) 
        {
            if (OwnLobbyManager.Instance.ActiveSessions.TryGetValue(OwnerId, out var session))
            {
                GameEvents.OnPlayerStatsChanged.Invoke(session, next);
            }
        }
        else if(next.Association == AssociationType.Boss)
        {
            GameEvents.OnBossStatChanged.Invoke(next);
        }
    }
#endregion
}

