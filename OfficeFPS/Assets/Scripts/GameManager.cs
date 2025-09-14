using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MenuManager menuManager;

    public bool isPaused = false;
    void Awake()
    {
        isPaused = true;
        menuManager = GetComponent<MenuManager>();
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
}
