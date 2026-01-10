using UnityEngine;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine.AI;

public class TestEnemyScript : NetworkBehaviour
{
    [SerializeField] private List<Transform> patrolPoints;

    private NavMeshAgent _agent;
    private int positionIndex;

    // Netzwerkvariable für die Rotation
    [SerializeField] private Quaternion syncedRotation;

    public override void OnStartServer()
    {
        _agent = GetComponent<NavMeshAgent>();
        positionIndex = 0;
        _agent.SetDestination(patrolPoints[positionIndex].position);
        if (!IsServerInitialized)
        {
            _agent.enabled = false;
        }
    }

    void Update()
    {
        if (IsServerInitialized)
        {
            // Server: Rotation berechnen und synchronisieren
            if (_agent.velocity.sqrMagnitude > 0.01f)
            {
                Vector3 dir = _agent.velocity.normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
                transform.Rotate(0, 0, +90f);
                var rot = transform.rotation;
                syncedRotation = rot;
                SetRotationObserversRpc(rot);
            }

            if (!_agent.pathPending && _agent.remainingDistance <= 0.2f)
            {
                NextPosition();
            }
        }
        else
        {
            // Client: Rotation übernehmen
            transform.rotation = syncedRotation;
        }
    }

    private void NextPosition()
    {
        positionIndex = (positionIndex + 1) % patrolPoints.Count;
        _agent.SetDestination(patrolPoints[positionIndex].position);
    }

    // Rotation an alle Clients synchronisieren
    [ObserversRpc(BufferLast = true)]
    private void SetRotationObserversRpc(Quaternion rot)
    {
        syncedRotation = rot;
    }
}

