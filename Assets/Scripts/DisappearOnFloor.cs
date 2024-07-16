using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearOnFloor : MonoBehaviour
{
    [field: SerializeField] public ParticleSystem Smoke
    { get; set; }


    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            var main = Smoke.main;
            main.startColor = GetComponentInChildren<MeshRenderer>().material.color;
            Instantiate(Smoke, transform.position, Quaternion.Euler(Vector3.up));

            gameObject.SetActive(false);
        }
    }
}
