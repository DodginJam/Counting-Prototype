using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    // Overall GameState.
    public bool GameActive
    { get; set; } = false;
    public bool GamePaused
    { get; private set; } = false;

    // Level variables.
    public int WaveNumber
    { get; private set; }
    public bool WaveInProgress
    { get; private set; }
    public int ObjectsToSpawn
    { get; private set; }
    public float ObjectSpawnDelay
    { get; private set; }
    public float MidWaveDelay
    { get; private set; } = 7.0f;
    public List<int> WaveScoreRecord
    { get; private set; } = new List<int>();
    public string DifficultyOption
    { get; private set; }
    public int MinimumWavePassScore
    { get; private set; }
    public int NumberOfWaves
    { get; private set; }

    // SpawnManager reference.
    public SpawnManager SpawnManagerScript
    { get; private set; }

    // UI reference.
    public UI_Manager UIManagerScript
    { get; private set; }
    public CargoTracker CargoTrackerScript
    { get; set; }

    // TruckScript reference.
    public MoveTruck MoveTruckScript
    { get; private set; }

    // Camera reference.
    [field: SerializeField] public GameObject MainCamera
    { get; private set; }
    [field: SerializeField] public GameObject MainMenuCamera
    { get; private set; }

    // AudioManagetScript reference.
    public AudioManager AudioManagerScript
    { get; private set; }

    // Player Difficulty GameObject.
    [field: SerializeField] public GameObject PlayerEasy
    { get; private set; }
    [field: SerializeField] public GameObject PlayerMedium
    { get; private set; }
    [field: SerializeField] public GameObject PlayerHard
    { get; private set; }

    private void Awake()
    {
        SpawnManagerScript = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        UIManagerScript = GameObject.FindWithTag("UI").GetComponent<UI_Manager>();

        MoveTruckScript = FindAnyObjectByType<MoveTruck>();

        AudioManagerScript = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    void Start()
    {
        // Ensures the MainMenu appears first.
        MainMenuCamera.SetActive(true);
        MainCamera.SetActive(false);
        UIManagerScript.SetUIElementActive(UIManagerScript.GameStartUI, true);
        UIManagerScript.SetUIElementActive(UIManagerScript.GameActiveUI, false);
    }

    void Update()
    {
        StartWave();
    }

    public void StartGame() // Method called via the start game difficulty buttons in StartGameUI.
    {
        GameActive = true;
        WaveNumber = 0;
        UIManagerScript.SetUIElementActive(UIManagerScript.GameStartUI, false);
        UIManagerScript.SetUIElementActive(UIManagerScript.GameActiveUI, true);

        MainMenuCamera.SetActive(false);
        MainCamera.SetActive(true);
    }

    // Allows wave to progress to the next wave once a wave is no longer in progress and Game is active.
    void StartWave()
    {
        if (WaveInProgress == false && GameActive == true)
        {
            // Set up new Wave or successfully end game if all waves completed.
            if (WaveNumber < NumberOfWaves)
            {
                WaveInProgress = true;
                WaveNumber++;
                SetNewWave();
            }
            else
            {
                SetGameSuccessState();
            }
        }
    }

    // Set the game to non-active, display end of wave information for a time, and then remove it and then flag the wave as ended.
    public IEnumerator EndOFWave()
    {
        // The game restricts interaction and any gameplay element by setting game to false.
        // Display how well the user performed in certain round. If they caught above 50% of objects, they continue, else they fail.
        GameActive = false;
        UIManagerScript.SetUIElementActive(UIManagerScript.WaveEndText.gameObject, true);
        UIManagerScript.UpdateWaveEndText(CalculateEndOfWaveScore(ObjectsToSpawn, CargoTrackerScript.TotalCount));

        MoveTruckScript.SetTruckToMove((float)MidWaveDelay / 2, true, false);
        yield return new WaitForSeconds((float)MidWaveDelay / 2);

        MoveTruckScript.SetTruckToMove((float)MidWaveDelay / 2, false, true);
        yield return new WaitForSeconds((float)MidWaveDelay / 2);

        // Delete any Cargo gameobjects in scene on wave end.
        foreach (GameObject cargo in SpawnManagerScript.CurrentWaveObjects)
        {
            Destroy(cargo);
        }

        // Deactivate the WaveEnd text and update the WaveInProgress flag to mark end of the current wave.
        UIManagerScript.SetUIElementActive(UIManagerScript.transform.Find("GameActiveUI/WaveEndText").gameObject, false);
        WaveInProgress = false;

        // Checks if in the last wave the user completed, did they pass minimum score to be allowed to continue.
        if (WaveScoreRecord[WaveScoreRecord.Count - 1] >= MinimumWavePassScore)
        {
            GameActive = true;
        }
        else
        {
            SetGameOverState();
        }
    }

    // Each if ... else statement allows custom number of objects and spawn delay to be set for each level.
    void SetNewWave()
    {
        // Update any UI elements for new wave.
        CargoTrackerScript.ResetCounter();
        UIManagerScript.UpdateCargoToCollectText(0);
        UIManagerScript.UpdateWaveNumberText(WaveNumber, NumberOfWaves);

        // Modify the ObjectsToSpawn and the ObjectSpawnDelay so that the difficulty increases as the player progresses through waves.
        if (WaveNumber != 1)
        {
            int increaseObjectCountBy = 2;
            float decreaseSpawnDelayModifier = 0.96f;

            ObjectsToSpawn += increaseObjectCountBy;
            ObjectSpawnDelay *= decreaseSpawnDelayModifier;
        }

        SpawnManagerScript.InitiateNewWave(WaveNumber, ObjectsToSpawn, ObjectSpawnDelay);
    }

    public int CalculateEndOfWaveScore(float totalBoxesSent, float totalBoxesCaught)
    {
        int waveScore = Mathf.RoundToInt((totalBoxesCaught / totalBoxesSent) * 100);
        WaveScoreRecord.Add(waveScore);
        return waveScore;
    }

    // Called before the Start game method via the Difficulty / Start Game UI buttons.
    public void SetDifficultyAndPlay(string difficultyLevel)
    {
        if (difficultyLevel == "easy")
        {
            MinimumWavePassScore = 50;
            UIManagerScript.UpdateCurrentTargetInActive(MinimumWavePassScore);
            SpawnManagerScript.ModifyObjectsToSpawnToMatchPlayer(1);
            NumberOfWaves = 7;

            // Set the initial number of object to spawn and the spawnDelay time of objects spawning after one another based on difficulty.
            ObjectsToSpawn = 3;
            ObjectSpawnDelay = 1.8f;

            Instantiate(PlayerEasy);
        } 
        else if (difficultyLevel == "medium")
        {
            MinimumWavePassScore = 60;
            UIManagerScript.UpdateCurrentTargetInActive(MinimumWavePassScore);
            SpawnManagerScript.ModifyObjectsToSpawnToMatchPlayer(2);
            NumberOfWaves = 10;

            // Set the initial number of object to spawn and the spawnDelay time of objects spawning after one another based on difficulty.
            ObjectsToSpawn = 4;
            ObjectSpawnDelay = 1.7f;

            Instantiate(PlayerMedium);
        }
        else if (difficultyLevel == "hard")
        {
            MinimumWavePassScore = 70;
            UIManagerScript.UpdateCurrentTargetInActive(MinimumWavePassScore);
            SpawnManagerScript.ModifyObjectsToSpawnToMatchPlayer(3);
            NumberOfWaves = 12;

            // Set the initial number of object to spawn and the spawnDelay time of objects spawning after one another based on difficulty.
            ObjectsToSpawn = 5;
            ObjectSpawnDelay = 1.60f;

            Instantiate(PlayerHard);
        }
    }

    public void SetGameOverState()
    {
        UIManagerScript.SetUIElementActive(UIManagerScript.GameOverUI, true);
        UIManagerScript.SetUIElementActive(UIManagerScript.GameActiveUI, false);
        UIManagerScript.UpdateLastCollectionTargetAndTargetMetInGameOver(MinimumWavePassScore, WaveScoreRecord[WaveScoreRecord.Count - 1]);
    }

    public void SetGameSuccessState()
    {
        GameActive = false;
        UIManagerScript.SetUIElementActive(UIManagerScript.GameActiveUI, false);
        UIManagerScript.SetUIElementActive(UIManagerScript.GameSuccessUI, true);
        UIManagerScript.DisplayAllCompletedTargets(WaveScoreRecord.ToArray());
        AudioManagerScript.PlayOneShotSound(AudioManagerScript.GameSuccess);
    }

    // Reference in restart buttons within GameOver and GameSuccessUI restart buttons.
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool IsCurrentWaveBelowThreeQuartersOfMaxWaves()
    {
        return (float)WaveNumber / (float)NumberOfWaves <= 0.75f;
    }
}
