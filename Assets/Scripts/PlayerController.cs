using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Player Character.
    public float PlayerBoundry
    { get; private set; } = 10.0f;
    public Rigidbody PlayerRigidbody
    { get; private set; }
    [field: SerializeField] public float PlayerForce
    { get; private set; }
    [field: SerializeField] public float PlayerSpeedLimit
    { get; private set; }
    public Vector3 SpawnPosition
    { get; private set; } = new Vector3(0.0f, 2.0f, 0.0f);

    // Input.
    public float PlayerInputHorizontal
    { get; private set; }

    // GameState Variables Reference.
    public GameStateManager GameStateManagerScript
    { get; private set; }

    void Awake()
    {
        PlayerRigidbody = GetComponent<Rigidbody>();

        GameStateManagerScript = GameObject.FindWithTag("GameStateManager").GetComponent<GameStateManager>();
    }

    void Start()
    {
        PlayerForce = 750000;
        PlayerSpeedLimit = 20;

        transform.position = SpawnPosition;
    }

    void Update()
    {
        if (GameStateManagerScript.GameActive)
        {
            PlayerInputHorizontal = Input.GetAxisRaw("Horizontal");
        }
        else
        {
            PlayerInputHorizontal = 0;
        }
    }

    void FixedUpdate()
    {
        // Player Movement.
        PlayerRigidbody.AddRelativeForce(Vector3.forward * PlayerForce * PlayerInputHorizontal, ForceMode.Force);

        if (PlayerRigidbody.velocity.z > PlayerSpeedLimit || PlayerRigidbody.velocity.z < -PlayerSpeedLimit)
        {
            PlayerRigidbody.velocity = PlayerRigidbody.velocity.normalized * PlayerSpeedLimit;
        }
    }

    public void IgnoreEntranceCollision(bool ignore)
    {
        Collider[] PlayerColliders = GetComponentsInChildren<Collider>();
        Collider Entrance = GameObject.Find("Building/RightWallEntrance").GetComponent<Collider>();

        foreach (Collider collider in PlayerColliders)
        {
            Physics.IgnoreCollision(collider, Entrance, ignore);
        } 
    }
}

