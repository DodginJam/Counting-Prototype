using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAndReset : MonoBehaviour
{
    public Vector3 StartPosition
    { get; private set; } = new Vector3(90.0f, 0f, 26.4200001f);
    public Vector3 EndPosition
    { get; private set; } = new Vector3(-241.830002f, 0f, 26.4200001f);
    [field: SerializeField] public float Speed
    { get; private set; }

    public float RightSidedZ
    { get; private set; } = 22f;

    public float LeftSidedZ
    { get; private set; } = 31f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * Speed);
        
        if (transform.position.x < EndPosition.x)
        {
            transform.position = new Vector3(StartPosition.x, StartPosition.y, LeftSidedZ);
        }

        if (transform.position.x > StartPosition.x)
        {
            transform.position = new Vector3(EndPosition.x, EndPosition.y, RightSidedZ);
        }
    }
}
