using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.ScrollRect;

public class MoveTruck : MonoBehaviour
{
    public GameObject Player
    { get; private set; }
    public PlayerController PlayerControllerScript
    { get; private set; }
    public Vector3 StartPosition
    { get; private set; } = new Vector3(0, 0, 34);
    public Vector3 LastPlayerPosition
    { get; private set; }
    public Vector3 EndPosition
    { get; private set; }
    public Vector3 EndPositionOffSet
    { get; private set; } = new Vector3(0, 0f, 13.5f);

    // Lerping Values.
    public bool ReadyToMove
    { get; set; }
    public float ElaspedTime
    { get; private set; }
    public float MovementTime
    { get; private set; }
    public bool TowardPlayer
    { get; private set; }

    public SpawnManager SpawnManagerScript
    { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        SpawnManagerScript = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // If the ReadyToMove flag is set to true, it allows the truck to Lerp from the Start to End position. Flag is set to false once the truck has reached end position.
        if (ReadyToMove && Player != null)
        {
            // This updates the EndPosition if the Player position has changed since last frame, to ensure the despite player movement the Lerp will still end exactly at player.
            
            if (Player.transform.position != LastPlayerPosition && TowardPlayer)
            {
                EndPosition = new Vector3(0, 0, Player.transform.position.z) + EndPositionOffSet;
                LastPlayerPosition = Player.transform.position;
            }

            ElaspedTime += Time.deltaTime;
            float percentageOfMovement = ElaspedTime / MovementTime;
            transform.position = Vector3.Lerp(StartPosition, EndPosition, percentageOfMovement);

            if (percentageOfMovement >= 1)
            {
                ReadyToMove = false;
                StartPosition = new Vector3(0, 0, 34);

                SetPlayerAndCargoAsChildren(false);
            }
        }
    }

    // The start and end position are different depending on if the truck is moving towards player or not.
    // Set up the variables used on Lerping the trucks position, along with flagging the truck as ready to move to allow update function to run.
    public void SetTruckToMove(float timeForMovement, bool towardsPlayer, bool playerStartsAsChild)
    {
        if (Player == null || PlayerControllerScript == null)
        {
            Player = GameObject.FindWithTag("Player");
            PlayerControllerScript = Player.GetComponent<PlayerController>();
        }

        
        if (towardsPlayer && !playerStartsAsChild) // For bringing the truck to the current Player position.
        {
            transform.position = StartPosition;
            EndPosition = new Vector3(0, 0, Player.transform.position.z) + EndPositionOffSet;

            TowardPlayer = true;
        }
        else if (towardsPlayer && playerStartsAsChild) // For moving the truck, and the attached player, to the StartPosition.
        {
            SetPlayerAndCargoAsChildren(true);
            TowardPlayer = false;

            transform.position = StartPosition;
            EndPosition = new Vector3(0, 0, PlayerControllerScript.SpawnPosition.z) + EndPositionOffSet;
        }
        else if (!towardsPlayer && playerStartsAsChild) // For bringing the truck away from the Player positon, with the player attached to truck.
        {
            EndPosition = StartPosition;
            StartPosition = transform.position;

            TowardPlayer = false;
            SetPlayerAndCargoAsChildren(true);
        }
        else if (!towardsPlayer && !playerStartsAsChild) // For bringing the truck away from the Player positon.
        {
            EndPosition = StartPosition;
            StartPosition = transform.position;

            TowardPlayer = false;
        }

        LastPlayerPosition = Player.transform.position;

        ElaspedTime = 0;
        MovementTime = timeForMovement;

        ReadyToMove = true;
    }

    void SetPlayerAndCargoAsChildren(bool setAsChildren)
    {
        if (setAsChildren)
        {
            // The Player is set as a child of the truck to allow it to be move by the trucks positional transform.
            // The Cargo is also as children of the truck, however these don't need to be detached later as they will be destroyed at the end of the round.
            Player.transform.SetParent(transform);

            foreach (GameObject cargo in Player.GetComponentInChildren<CargoTracker>().CargoCarriedByPlayer)
            {
                cargo.transform.SetParent(transform);
                cargo.GetComponent<Rigidbody>().isKinematic = true;
            }

            PlayerControllerScript.IgnoreEntranceCollision(true);
        }
        else
        {
            // The Player is removed as a child of the truck to allow it regain independent movement.
            Player.transform.parent = null;
            PlayerControllerScript.IgnoreEntranceCollision(false);
        }
    }

}
