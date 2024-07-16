using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.ScrollRect;

public class MoveCrane : MonoBehaviour
{
    public Vector3 StartPosition
    { get; private set; }
    public Vector3 CurrentPosition
    { get; private set; }
    public Vector3 EndPosition
    { get; private set; }

    public float ElaspedTime
    { get; private set; }
    public float MovementTime
    { get; private set; }

    public bool ReadyToMove
    { get; private set; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // If the crane object ReadyToMove flag is set to true, it allows the Crane's position to Lerp from the Start to End position.
        if (ReadyToMove)
        {
            ElaspedTime += Time.deltaTime;
            float percentageOfMovement = ElaspedTime / MovementTime;
            transform.position = Vector3.Lerp(StartPosition, EndPosition, percentageOfMovement);

            if (percentageOfMovement >= 1)
            {
                ReadyToMove = false;
            }
        }
    }

    // Movement time is ideally the spawn time between objects, allowing the crane to move between spawn positions within the spawn delay durations.
    public void StartCraneMovement(float movementTime, Vector3 objectToMoveToPosition)
    {
        StartPosition = transform.position;
        EndPosition = new Vector3(transform.position.x, transform.position.y, objectToMoveToPosition.z);

        ElaspedTime = 0;

        // This value is taken from from the movementTime value, so that the crane object will appear at the spawn point location for this time before the object spawns.
        float movementTimeReducedBy = 0.5f;

        // This ensures that if the "movementTime" is less then the "movementTimeReducedBy", it just sets movementTime to "movementTimeReducedBy".
        if (movementTime > movementTimeReducedBy)
        {
            MovementTime = movementTime - movementTimeReducedBy;
        }
        else
        {
            MovementTime = movementTimeReducedBy;
        }

        ReadyToMove = true;
    }
}
