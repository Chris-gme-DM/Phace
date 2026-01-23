using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    public readonly int _projectileDataID;
    public AssociationType _associationType;    // should inherit this from the spacecraft it is fired from
    public readonly SyncVar<ProjectileStats> Stats = new();
}
