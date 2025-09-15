using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScenes : MonoBehaviour
{
    public GameObject LoadingScreen;
    public Image LoadingBar;

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        LoadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);

            LoadingBar.fillAmount = progressValue;

            // Red -> Green -> Blue gradient
            if (progressValue <= 0.5f)
            {
                float t = progressValue / 0.5f;
                LoadingBar.color = Color.Lerp(Color.red, Color.green, t);
            }
            else
            {
                float t = (progressValue - 0.5f) / 0.5f;
                LoadingBar.color = Color.Lerp(Color.green, Color.blue, t);
            }

            yield return null;
        }
    }

    public void RelaodScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"[LoadingScenes] Reloading scene: {currentSceneName}");
        LoadScene(currentSceneName);
    }
}
