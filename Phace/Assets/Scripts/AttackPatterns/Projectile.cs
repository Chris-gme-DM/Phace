using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    public readonly ProjectileData _projectileData;
    public AssociationType _associationType;
    public readonly SyncVar<ProjectileStats> Stats = new();
}
