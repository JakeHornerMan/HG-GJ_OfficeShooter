using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public MenuManager menuManager;
    public HudManager hudManager;
    public GameObject player;
    public PlayerSounds playerSound;
    private LoadingScenes loadingScenes;

    [Header("Scene Changing")]
    public string nextSceneName;

    [Header("KeyCards")]
    public bool needKeyCard = false;
    private bool hasKeyCard = false;

    [Header("Package PickUp")]
    public bool needToPickUpPackage = false;
    private bool hasPickedUpPackage = false;

    [Header("Package Delivery")]
    public bool needToDeliverPackage = false;
    private bool hasDeliveredPackage = false;

    [Header("Mission Log")]
    public List<string> missionLogs = new List<string>();

    [Header("Pause Screen")]
    public bool isPaused = false;

    void Awake()
    {
        isPaused = true;
        hasKeyCard = false;
        hasPickedUpPackage = false;
        hasDeliveredPackage = false;
        SetStartMissionLog();
        menuManager = GetComponent<MenuManager>();
        playerSound = player.GetComponent<PlayerSounds>();
        loadingScenes = GetComponent<LoadingScenes>();
        playerSound.PlayEnterLevel();
    }

    void Start()
    {
        InformPlayerHud("Jake Test Area");    
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseGamne();
        }
    }

    private void PauseGamne()
    {
        Time.timeScale = 0f;
    }

    private void UnpauseGamne()
    {
        Time.timeScale = 1f;
    }

    public void LoadNextScene()
    {
        // If keycard is required but not collected
        if (needKeyCard && !hasKeyCard)
        {
            Debug.Log("[GameManager] Cannot load scene: keycard is required but not collected.");
            InformPlayerHud("Find the Key Card");
            return;
        }

        // If package pickup is required but not collected
        if (needToPickUpPackage && !hasPickedUpPackage)
        {
            Debug.Log("[GameManager] Cannot load scene: package pickup is required but not completed.");
            InformPlayerHud("Collect the Package");
            return;
        }

        // If package delivery is required but not completed
        if (needToDeliverPackage && !hasDeliveredPackage)
        {
            Debug.Log("[GameManager] Cannot load scene: package delivery is required but not completed.");
            InformPlayerHud("Deliver the Package");
            return;
        }

        // If any requirement is satisfied
        if ((needKeyCard && hasKeyCard) ||
            (needToPickUpPackage && hasPickedUpPackage) ||
            (needToDeliverPackage && hasDeliveredPackage))
        {
            playerSound.PlayLeaveLevel();
            
            Invoke(nameof(LoadScene), 1f);
            return;
        }

        // If no requirements are active at all
        Debug.Log("[GameManager] No requirements set, loading next scene by default.");
        SceneManager.LoadScene(nextSceneName);
    }

    private void LoadScene()
    {
        Debug.Log($"[GameManager] Requirements met, loading scene: {nextSceneName}");
        
        loadingScenes.LoadScene(nextSceneName);
    }

    public void GotKeyCard()
    {
        hasKeyCard = true;
        Debug.Log($"[GameManager] GotKeyCard, hasKeyCard: {hasKeyCard}");
    }

    public void PickedUpPackage()
    {
        hasPickedUpPackage = true;
        Debug.Log($"[GameManager] PickedUpPackage, hasPickedUpPackage: {hasPickedUpPackage}");
    }

    public void DeliveredUpPackage()
    {
        hasDeliveredPackage = true;
        Debug.Log($"[GameManager] DeliveredUpPackage, hasDeliveredPackage: {hasPickedUpPackage}");
    }

    public void AddToMissionLogs(string logEntry)
    {
        missionLogs.Add(logEntry);
        Debug.Log($"[GameManager] Added mission log: {logEntry}");
        hudManager.InformPlayer(logEntry);
    }

    public void SetStartMissionLog()
    {
        if (needKeyCard)
        {
            AddToMissionLogs("You need to find the keycard to progress to the next floor");
            return;
        }

        if (needToPickUpPackage)
        {
            AddToMissionLogs("Pick Up your package to start the mission!");
            return;
        }

        if (needToDeliverPackage)
        {
            AddToMissionLogs("You have made it to the correct floor lets deliver that package!");
            return;
        }
    }

    public void InformPlayerHud(string text)
    {
        playerSound.PlayInformPlayer();
        hudManager.InformPlayer(text);
    }
}
