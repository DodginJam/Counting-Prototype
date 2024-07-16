using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CargoTracker : MonoBehaviour
{
    public int TotalCount
    { get; private set; } = 0;

    // UI varibles.
    public UI_Manager UIManagerScript
    { get; private set; }

    //
    public GameStateManager GameStateManagerScript
    { get; private set; }

    // Tracking All Cargo Objects within Player.
    public List<GameObject> CargoCarriedByPlayer
    { get; private set; } = new List<GameObject>();

    private void Awake()
    {
        UIManagerScript = GameObject.FindWithTag("UI").GetComponent<UI_Manager>();

        GameStateManagerScript = GameObject.FindWithTag("GameStateManager").GetComponent<GameStateManager>();

        GameStateManagerScript.CargoTrackerScript = this;
    }

    private void Start()
    {
        TotalCount = 0;
    }

    public void ModifyTotalCount(int totalToAdd)
    {
        TotalCount += totalToAdd;
        UIManagerScript.UpdateCargoCollectedText(TotalCount);
    }

    public void ModifyCargoCarriedByPlayer(GameObject cargo, bool toAdd)
    {
        if (toAdd)
        {
            CargoCarriedByPlayer.Add(cargo);
        }
        else
        {
            CargoCarriedByPlayer.Remove(cargo);
        }
    }

    // Reset the total counter and the indivdual cargo counters.
    public void ResetCounter()
    {
        TotalCount = 0;
        UIManagerScript.UpdateCargoCollectedText(TotalCount);

        CargoCounter[] cargoCounters = GetComponentsInChildren<CargoCounter>();
        foreach (CargoCounter counter in cargoCounters)
        {
            counter.ResetCounter();
        }

        CargoCarriedByPlayer = new List<GameObject>();
    }
}
