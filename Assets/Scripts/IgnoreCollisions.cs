using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollisions : MonoBehaviour
{
    public Collider WallToIgnore
    { get; private set; }

    private void Awake()
    {
        WallToIgnore = GameObject.FindWithTag("CargoToIgnore").GetComponent<Collider>();
    }

    void Start()
    {
        Physics.IgnoreCollision(WallToIgnore, gameObject.GetComponent<Collider>());
    }
}
