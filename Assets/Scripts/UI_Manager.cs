using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    // UI collections GameObjects.
    public GameObject GameActiveUI
    { get; private set; }
    public GameObject GameStartUI
    { get; private set; }
    public GameObject GameOverUI
    { get; private set; }
    public GameObject GameSuccessUI
    { get; private set; }

    // UI Text during Start Game.
    public UnityEngine.UI.Slider VolumeSlider
    { get; private set; }

    // UI Text during Active Game.
    public TextMeshProUGUI CargoCollectedText
    { get; private set; }
    public TextMeshProUGUI CargoToCollectText
    { get; private set; }
    public TextMeshProUGUI WaveEndText
    { get; private set; }
    public TextMeshProUGUI WaveNumberText
    { get; private set; }
    public TextMeshProUGUI AlertText
    { get; private set; }
    public TextMeshProUGUI CollectionTargetDisplayText
    { get; private set; }
    [field: SerializeField] public GameObject SpawnWarningDisplay
    { get; private set; }

    // UI Text during Game Over.
    public TextMeshProUGUI CollectionTargetText
    { get; private set; }
    public TextMeshProUGUI TargetMetText
    { get; private set; }

    // UI Text during Game Success.
    public TextMeshProUGUI WaveTargetsMetText
    { get; private set; }

    // Sprites.
    [field: SerializeField] public Sprite[] WarningDisplaySprites
    { get; private set; }
    [field: SerializeField] public Sprite ErrorDisplaySprite
    { get; private set; }

    // Audi Manager Reference.
    public AudioManager AudioManagerScript
    { get; private set; }

    private void Awake()
    {
        GameActiveUI = transform.Find("GameActiveUI").gameObject;
        GameStartUI = transform.Find("GameStartUI").gameObject;
        GameOverUI = transform.Find("GameOverUI").gameObject;
        GameSuccessUI = transform.Find("GameSuccessUI").gameObject;

        CargoCollectedText = transform.Find("GameActiveUI/CargoCollectedText").GetComponent<TextMeshProUGUI>();
        CargoToCollectText = transform.Find("GameActiveUI/CargoToCollectText").GetComponent<TextMeshProUGUI>();
        WaveEndText = transform.Find("GameActiveUI/WaveEndText").GetComponent<TextMeshProUGUI>();
        WaveNumberText = transform.Find("GameActiveUI/WaveNumberText").GetComponent<TextMeshProUGUI>();
        AlertText = transform.Find("GameActiveUI/AlertText").GetComponent<TextMeshProUGUI>();
        CollectionTargetDisplayText = transform.Find("GameActiveUI/CollectionTarget/CollectionTargetDisplayText").GetComponent<TextMeshProUGUI>();

        CollectionTargetText = transform.Find("GameOverUI/CollectionTargetText").GetComponent<TextMeshProUGUI>();
        TargetMetText = transform.Find("GameOverUI/TargetMetText").GetComponent<TextMeshProUGUI>();

        WaveTargetsMetText = transform.Find("GameSuccessUI/WaveTargetsMetText").GetComponent<TextMeshProUGUI>();

        AudioManagerScript = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        VolumeSlider = GameStartUI.transform.Find("VolumeSlider").GetComponent<UnityEngine.UI.Slider>();
    }

    void Start()
    {
        GameStartUI.SetActive(true);
        GameActiveUI.SetActive(false);
        GameOverUI.SetActive(false);
        GameSuccessUI.SetActive(false);

        SetUpVolumeSlider();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUIElementActive(GameObject UIElement, bool setActive)
    {
        if (setActive)
        {
            UIElement.SetActive(true);
        }
        else
        {
            UIElement.SetActive(false);
        }
    }

    public void UpdateWaveEndText(int newCount)
    {
        WaveEndText.text = $"Cargo Sorted For Delivery:\n{newCount}%";
    }

    public void UpdateCargoCollectedText(int newCount)
    {
        CargoCollectedText.text = $"{newCount} - Cargo Sorted";
    }

    public void UpdateCargoToCollectText(int newCount)
    {
        CargoToCollectText.text = $"Cargo To Collect - {newCount}";
    }

    public void UpdateWaveNumberText(int currentWave, int totalWaves)
    {
        WaveNumberText.text = $"Wave Number: {currentWave}/{totalWaves}";
    }

    // Display any message for a set amount of time to the AlertText UI element.
    public IEnumerator FlashAlert(string message, float secondsToDisplay)
    {
        AlertText.text = message;
        AlertText.gameObject.SetActive(true);
        yield return new WaitForSeconds(secondsToDisplay);
        AlertText.gameObject.SetActive(false);
    }

    // Can be used to set activate or deactivate the Spawn Warning UI display, whilst updating it's location to apppear relative to the spawning gameobject.
    public void SpawnWarningSetDisplay(bool setDisplay, GameObject objectToDisplay, bool checkName)
    {
        if (setDisplay == true)
        {
            SpawnWarningDisplay.SetActive(true);

            Sprite SpriteToDisplay = null;

            if (checkName)
            {
                if (objectToDisplay.name.Contains("Cube1"))
                {
                    SpriteToDisplay = WarningDisplaySprites[0];
                }
                else if (objectToDisplay.name.Contains("Cube2"))
                {
                    SpriteToDisplay = WarningDisplaySprites[1];
                }
                else if (objectToDisplay.name.Contains("Cube3"))
                {
                    SpriteToDisplay = WarningDisplaySprites[2];
                }
            }
            else if (!checkName)
            {
                SpriteToDisplay = ErrorDisplaySprite;
            }

            SpawnWarningDisplay.GetComponent<UnityEngine.UI.Image>().sprite = SpriteToDisplay;
        }
        else
        {
            SpawnWarningDisplay.SetActive(false);
        }
    }

    public void UpdateLastCollectionTargetAndTargetMetInGameOver(int collectionTarget, int targetMet)
    {
        CollectionTargetText.text = $"Collection Target:\t{collectionTarget}%";
        TargetMetText.text = $"Your Target Met:\t{targetMet}%";
    }

    public void UpdateCurrentTargetInActive(int collectionTarget)
    {
        CollectionTargetDisplayText.text = $"{collectionTarget}%";
    }

    public void DisplayAllCompletedTargets(int[] targetsAcheived)
    {
        WaveTargetsMetText.text = string.Empty;

        for (int i = 0; i < targetsAcheived.Length; i++)
        {
            string formatedString = $"Wave {i + 1}: {targetsAcheived[i]}%";

            WaveTargetsMetText.text += formatedString;

            // If the Wave isn't the last Wave, add additional spacing options at the end.
            // Any wave divisable by six and equalling zero has new line added to stop text from going off screen.
            if (i < targetsAcheived.Length - 1 && (i + 1) % 6 != 0)
            {
                WaveTargetsMetText.text += " | ";
            }
            else
            {
                WaveTargetsMetText.text += " |\n";
            }
        }
    }

    public void SetUpVolumeSlider()
    {
        VolumeSlider.maxValue = AudioManagerScript.MaximumVolumeMultiplier;
        VolumeSlider.minValue = 0;
        VolumeSlider.value = 1;
        VolumeSlider.onValueChanged.AddListener(newValue => AudioManagerScript.ChangeVolume(newValue));
    }
}
