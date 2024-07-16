using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CargoCounter : MonoBehaviour
{
    public int Count
    { get; private set; } = 0;
    public CargoTracker CargoTrackerScript
    { get; private set; }
    [field: SerializeField] public GameObject CargoToCount
    { get; private set; }

    // AudioManagetScript reference.
    public AudioManager AudioManagerScript
    { get; private set; }
    // AudioManagetScript reference.
    public GameStateManager GameStateManagerScript
    { get; private set; }

    private void Awake()
    {
        CargoTrackerScript = GetComponentInParent<CargoTracker>();
        AudioManagerScript = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        GameStateManagerScript = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();
    }

    private void Start()
    {
        Count = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains($"{CargoToCount.name}"))
        {
            Count += 1;
            CargoTrackerScript.ModifyTotalCount(1);
            CargoTrackerScript.ModifyCargoCarriedByPlayer(other.gameObject, true);
            if (GameStateManagerScript.GameActive == true)
            {
                AudioManagerScript.PlayOneShotSound(AudioManagerScript.ObjectGained);
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Contains($"{CargoToCount.name}"))
        {
            Count -= 1;
            CargoTrackerScript.ModifyTotalCount(-1);
            CargoTrackerScript.ModifyCargoCarriedByPlayer(other.gameObject, false);
            if (GameStateManagerScript.GameActive == true)
            {
                AudioManagerScript.PlayOneShotSound(AudioManagerScript.ObjectLost);
            }
        }
    }

    public void ResetCounter()
    {
        Count = 0;
    }
}
