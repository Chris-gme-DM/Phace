using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

// Inherit from NetworkBehaviour instead of MonoBehaviour
public class PlayerMovement : NetworkBehaviour
{
    // SyncVar that synchronizes the player's movement speed across the network
    public readonly SyncVar<float> syncSpeed = new SyncVar<float>();

    private Vector2 _input;

    private void OnDisable()
    {
        // Unsubscribe from tick event to avoid leaks
        if (TimeManager != null)
        {
            TimeManager.OnTick -= TimeManager_OnTick;
        }
    }

    private void Start()
    {
        // Register callback when the SyncVar changes
        syncSpeed.OnChange += OnSpeedChange;

        // Subscribe to tick event when the object is active
        TimeManager.OnTick += TimeManager_OnTick;

        // Initialize default speed on the server (if not already set)
        if (syncSpeed.Value == 0f)
            syncSpeed.Value = 5f;
    }

    /// <summary>
    /// Called by FishNet's TimeManager on every network tick.
    /// </summary>
    private void TimeManager_OnTick()
    {
        // Only the owning client should read input
        if (!IsOwner)
            return;
        Camera cam = Camera.main;
        if (cam == null)
            return;

        // Make sure we actually have a keyboard (e.g. not on some weird platform)
        var keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        // --- WASD movement using the new Input System ---
        float horizontal = 0f;
        float vertical = 0f;

        if (keyboard.aKey.isPressed) horizontal -= 1f;
        if (keyboard.dKey.isPressed) horizontal += 1f;
        if (keyboard.sKey.isPressed) vertical -= 1f;
        if (keyboard.wKey.isPressed) vertical += 1f;

        _input = new Vector2(horizontal, vertical);

        // M key toggles speed (new Input System)
        if (keyboard.mKey != null && keyboard.mKey.wasPressedThisFrame)
            ChangeSpeed();

        // Send input to the server (server-authoritative movement)
        if (_input != Vector2.zero)
            MoveServer(_input);

        Vector3 mouseScreen = Mouse.current.position.ReadValue();

        float depth = Mathf.Abs(cam.transform.position.z - transform.position.z);

        Vector3 mouseWorld = cam.ScreenToWorldPoint(
            new Vector3(mouseScreen.x, mouseScreen.y, depth)
        );

        Vector2 lookDir = mouseWorld - transform.position;

        if (lookDir.sqrMagnitude > 0.001f)
            LookServer(lookDir);

    }

    [ServerRpc]
    private void MoveServer(Vector2 input)
    {
        // Use TickDelta for tick-based movement instead of Time.deltaTime
        float delta = (float)TimeManager.TickDelta;

        // Calculate movement on the server only (server-authoritative)
        Vector2 movement = input.normalized * syncSpeed.Value * delta;

        // Apply movement to server-side position (2D: only x and y)
        transform.position += new Vector3(movement.x, movement.y, 0f);

        // Create callback message
        string callbackText = $"Moved by: {movement}";

        // Send callback only to the owning client
        MoveCallback(Owner, callbackText);
        

    
    }

    [ServerRpc]
    private void LookServer(Vector2 lookDir)
    {
        // Calculate rotation angle in degrees
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        // Apply rotation to server-side transform
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        transform.Rotate(0, 0, -90f); // Adjust for sprite facing direction
    }

    // First parameter MUST be NetworkConnection for a TargetRpc
    [TargetRpc]
    private void MoveCallback(NetworkConnection conn, string msg)
    {
        // Runs only on the client that owns this object
        Debug.Log($"[Callback] {msg}");
    }

    [ServerRpc]
    private void ChangeSpeed()
    {
        // Toggle between two speeds (server decides)
        syncSpeed.Value = syncSpeed.Value == 5f ? 10f : 5f;
    }

    public void OnSpeedChange(float prev, float next, bool asServer)
    {
        // Logs whenever the speed SyncVar changes
        Debug.Log($"Speed changed: {prev} → {next}");
    }
}