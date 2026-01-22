using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using System;
using System.Collections;

public class EnemySpawnManager : NetworkBehaviour
{
    [SerializeField] public List<Transform> patrolPointsA;
    [SerializeField] public List<Transform> patrolPointsB;
    [SerializeField] public List <Transform> patrolPointsC;
    [SerializeField] private NetworkObject enemyTypeA;
    [SerializeField] private NetworkObject enemyTypeB;
    [SerializeField] private Transform[] spawnPointsA;
    [SerializeField] private Transform[] spawnPointsB;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int spawnLimitA = 20;
    [SerializeField] private int spawnLimitB = 20;
    private float spawnReset;
  
    private List<NetworkObject> activeEnemiesA = new List<NetworkObject>();
    private List<NetworkObject> activeEnemiesB = new List<NetworkObject>();
    private int spawnedAmountA = 0;
    private int spawnedAmountB = 0;
    private bool waveAComplete = false;
    private bool waveBComplete = false;
    private bool waveADestroyed = false;
    private bool waveBDestroyed = false;
    private bool secondWaveDelayed = false;
    private bool secondWaveCanStart = false;

    public static EnemySpawnManager Instance;
    // Additions
    private bool isPlaying = false;
    public override void OnStartServer()
    {
        spawnReset = spawnInterval;
        base.OnStartServer();
        
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        TimeManager.OnTick += OnTick;

        GameEvents.OnGameStateChanged.AddListener(HandleGameStateChanged);
    }

    private void HandleGameStateChanged(GameState newState)
    {
        if (newState != GameState.InGame) isPlaying = false;
        if (newState == GameState.InGame) isPlaying = true;
    }

    private void Update()
    {
        if (!isPlaying)
            return;

        if (spawnedAmountA >= spawnLimitA)
        {
            waveAComplete = true;
        }
        
        if (spawnedAmountB >= spawnLimitB)
        {
            waveBComplete = true;
        }

        if ( activeEnemiesA.Count == 0 && waveAComplete && !waveADestroyed)
        {

            waveADestroyed = true;
        }
        else if ( activeEnemiesB.Count == 0 && waveBComplete && !waveBDestroyed)
        {
            waveBDestroyed = true;
        }
        Debug.Log( waveAComplete );
        Debug.Log( waveADestroyed ); 
        Debug.Log(activeEnemiesA.Count);
        Debug.Log(activeEnemiesB.Count);
    }

    private void OnTick()
    {
        if (!IsServerInitialized)
            return;
        float tickDelta = (float)TimeManager.TickDelta;
        spawnInterval -= tickDelta;
        if (spawnInterval <= 0f)
        {
            SpawnEnemies();
            spawnInterval = spawnReset; 
        }
    }

    [Server]

    private void SpawnEnemies()
    {
        if (!waveAComplete)
        {
            spawnedAmountA++;


            int spawnIndex = UnityEngine.Random.Range(0, spawnPointsA.Length);
            NetworkObject enemyObj = Instantiate(enemyTypeA, spawnPointsA[spawnIndex].position, Quaternion.identity);


            Spawn(enemyObj);
            activeEnemiesA.Add(enemyObj);
        }
        else if (waveAComplete && waveADestroyed && !waveBComplete)
        {
            if (!secondWaveDelayed)
            {
                StartCoroutine(DelaySecondWave());
                secondWaveDelayed = true;
            }
            
            if (!secondWaveCanStart) return;
            spawnedAmountB++;
            int spawnIndex = UnityEngine.Random.Range(0, spawnPointsB.Length);
            NetworkObject enemyObj = Instantiate(enemyTypeB, spawnPointsB[spawnIndex].position, Quaternion.identity);


            Spawn(enemyObj);
            activeEnemiesB.Add(enemyObj);
        }
        else if (waveBComplete && waveBDestroyed)
        {
            // All waves complete
            TimeManager.OnTick -= OnTick; // Stop spawning
        }
    }
    [Server]
    public void NotifyEnemyDestroyed(NetworkObject enemy)
    {
        if (activeEnemiesA.Remove(enemy)) return;
        if (activeEnemiesB.Remove(enemy)) return;
        GameEvents.OnEnemyDestroyed.Invoke();
    }

    [Server]
    IEnumerator DelaySecondWave()
    {
        
        yield return new WaitForSeconds(7f);
        secondWaveCanStart = true;
    }

}


