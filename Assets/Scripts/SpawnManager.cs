using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public MeshRenderer SpawnArea
    { get; private set; }
    public float SpawnDelay
    { get; private set; }
    public float DelayAfterLastSpawn
    { get; private set; } = 5.0f;
    public int NumOfObjectsToSpawn
    { get; private set; }
    [field: SerializeField] public GameObject[] ObjectsToSpawn
    { get; private set; }
    public GameObject[] CurrentWaveObjects
    { get; private set; }
    public bool ReadyToSpawn
    { get; private set; }

    // GameState Variables Reference.
    public GameStateManager GameStateManagerScript
    { get; private set; }

    // UI Variables Reference.
    public UI_Manager UI_ManagerScript
    { get; private set; }

    // CraneObject Reference.
    public MoveCrane MoveCraneScript
    { get; private set; }

    // MoveTruck Reference.
    public MoveTruck MoveTruckScript
    { get; private set; }

    private void Awake()
    {
        SpawnArea = transform.Find("SpawnArea").GetComponent<MeshRenderer>();
        MoveCraneScript = transform.Find("SpawnCrane").GetComponent<MoveCrane>();

        GameStateManagerScript = GameObject.FindWithTag("GameStateManager").GetComponent<GameStateManager>();
        UI_ManagerScript = GameObject.FindWithTag("UI").GetComponent<UI_Manager>();

        MoveTruckScript = FindObjectOfType<MoveTruck>();
    }

    void Start()
    {

    }

    void Update()
    {
        StartWaveSpawning();
    }

    // If the ready to spawn flag is set to true, it starts the coroutine to allow objects to spawn and fall from above.
    public void StartWaveSpawning()
    {
        if (ReadyToSpawn == true)
        {
            CurrentWaveObjects = LoadObjectsForWave(NumOfObjectsToSpawn);
            StartCoroutine(SpawnObjectsAtInterval(CurrentWaveObjects, SpawnDelay));
            ReadyToSpawn = false;
        }
    }

    // Allows the GameStateManager, upon a new wave, to update the number of objects to spawn and the spawn delay between objects, and provide these to the SpawnManager. Then set ReadyToSpawn flag to true.
    public void InitiateNewWave(int waveNumber,int numObjectsToSpawn, float ObjectSpawnDelay)
    {
        NumOfObjectsToSpawn = numObjectsToSpawn;
        SpawnDelay = ObjectSpawnDelay;
        ReadyToSpawn = true;
    }

    // Returns a GameObject containing the pre-loaded, but not yet active, objects that are to fall and be collected by the player per wave.
    GameObject[] LoadObjectsForWave(int numberOfObjects)
    {
        GameObject[] objectsToLoad = new GameObject[numberOfObjects];

        for (int i = 0; i < objectsToLoad.Length; i++)
        {
            int objectIndex = Random.Range(0, ObjectsToSpawn.Length);

            objectsToLoad[i] = Instantiate(ObjectsToSpawn[objectIndex], GenerateSpawnPosition(), ObjectsToSpawn[objectIndex].transform.rotation);
            objectsToLoad[i].SetActive(false);
        }

        UI_ManagerScript.UpdateCargoToCollectText(numberOfObjects);

        return objectsToLoad;
    }

    // Allows the pre-loaded objects to spawn at intervals from one another.
    IEnumerator SpawnObjectsAtInterval(GameObject[] objectsToSpawn, float spawnDelay)
    {
        int objectIndexToSpawn = 0;

        // While loop ensures that the Coroutine keeps allowing objects to fall for as many as there exist in the array.
        while (objectIndexToSpawn < objectsToSpawn.Length)
        {
            // Display message before first object of a wave is spawned and allow truck to move player back into view if a prior wave finished.
            if (objectIndexToSpawn == 0)
            {
                float startedWaveDelay = 7.0f;
                StartCoroutine(UI_ManagerScript.FlashAlert("New Order for Delivery", startedWaveDelay));

                // Below IF statement allows game to hold interaction and allow the truck to return player to the start position from beyond Wave 1.
                // The reason for the exception of round one is that the player starts already in the Start Position on round 1.
                if (GameStateManagerScript.WaveNumber > 1)
                {
                    // Setting the game active to false tempoarily will disable the controls until set back to active after wait.
                    GameStateManagerScript.GameActive = false;
                    MoveTruckScript.SetTruckToMove((float)startedWaveDelay / 2, true, true);
                    yield return new WaitForSeconds((float)startedWaveDelay / 2);

                    MoveTruckScript.SetTruckToMove((float)startedWaveDelay / 2, false, false);
                    yield return new WaitForSeconds((float)startedWaveDelay / 2);
                    GameStateManagerScript.GameActive = true;
                }
                else
                {
                    yield return new WaitForSeconds((float)startedWaveDelay);
                }

            }

            // Displays the SpawnWarning UI element for duration before spawn, and also starts the crane movement.
            // Spawn Warning display until three-quarters of Waves Completed, then it is disabled.
            if (GameStateManagerScript.IsCurrentWaveBelowThreeQuartersOfMaxWaves())
            {
                UI_ManagerScript.SpawnWarningSetDisplay(true, objectsToSpawn[objectIndexToSpawn], true);
            }
            else
            {
                UI_ManagerScript.SpawnWarningSetDisplay(true, objectsToSpawn[objectIndexToSpawn], false);
            }

            MoveCraneScript.StartCraneMovement(spawnDelay, objectsToSpawn[objectIndexToSpawn].transform.position);

            yield return new WaitForSeconds(spawnDelay);

            UI_ManagerScript.SpawnWarningSetDisplay(false, objectsToSpawn[objectIndexToSpawn], false);

            objectsToSpawn[objectIndexToSpawn].SetActive(true);
            objectIndexToSpawn++;
        }

        // Only once the last object has spawned, and after a small delay, is the Wave ending process then started.
        yield return new WaitForSeconds(DelayAfterLastSpawn);
        StartCoroutine(GameStateManagerScript.EndOFWave());
    }

    // Generates a spawn position for the falling objects based upon the size of the spawn area object - only variation along X and Z axis.
    Vector3 GenerateSpawnPosition()
    {
        float halfOfZ = SpawnArea.bounds.size.z / 2;
        float lowerBoundZ = SpawnArea.bounds.center.z + halfOfZ;
        float upperBoundZ = SpawnArea.bounds.center.z - halfOfZ;
        float randomZSpawn = Random.Range(lowerBoundZ, upperBoundZ);

        float halfOfX = SpawnArea.bounds.size.x / 2;
        float lowerBoundX = SpawnArea.bounds.center.x + halfOfX;
        float upperBoundX = SpawnArea.bounds.center.x - halfOfX;
        float randomXSpawn = Random.Range(lowerBoundX, upperBoundX);

        return new Vector3(randomXSpawn, SpawnArea.transform.position.y - 0.5f, randomZSpawn);
    }

    // Limits the number of objects allowed to spawn based on the difficulty - due to the Player GameObject accepts more cargo types with higher difficulty, therefore these cargo types should only spawn when they can be caught and tracked by player.
    public void ModifyObjectsToSpawnToMatchPlayer(int numberToSpawn)
    {
        GameObject[] newObjectsToSpawn = new GameObject[numberToSpawn];

        for (int i = 0; i < newObjectsToSpawn.Length; i++)
        {
            newObjectsToSpawn[i] = ObjectsToSpawn[i];
        }

        ObjectsToSpawn = newObjectsToSpawn;
    }
}
