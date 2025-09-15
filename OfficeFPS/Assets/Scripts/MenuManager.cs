using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private LoadingScenes loadingScenes;
    public GameObject pauseMenu;

    private void Awake()
    {
        loadingScenes = GetComponent<LoadingScenes>();
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        Debug.Log("[MenuManager] PauseGame");
        GameManager.Instance.isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        Debug.Log("[MenuManager] ResumeGame");
        GameManager.Instance.isPaused = false;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        loadingScenes.RelaodScene();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("[MenuManager] RestartLevel");
    }
}
