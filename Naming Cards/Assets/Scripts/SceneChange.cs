using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    private GameManager gameManager;

    void Awake() {
		DontDestroyOnLoad(this.gameObject);
        GameObject gameManagerObj = GameObject.Find("GameManager");
        gameManager = gameManagerObj.GetComponent<GameManager>();
	}

    public void PlayGame() {
        StartCoroutine(LoadGame());
    }

    public void MainMenu() {
        SceneManager.LoadScene("MenuScene");
    }

    public void Gallery() {
        StartCoroutine(LoadGallery());
    }

    private IEnumerator LoadGame() {
        // Start loading the scene
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);
        // Wait until the level finish loading
        while (!asyncLoadLevel.isDone) yield return null;
        // Wait a frame so every Awake and Start method is called
        yield return new WaitForEndOfFrame();
        gameManager.StartGame();
    }

    private IEnumerator LoadGallery() {
        // Start loading the scene
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync("GalleryScene", LoadSceneMode.Single);
        // Wait until the level finish loading
        while (!asyncLoadLevel.isDone) yield return null;
        // Wait a frame so every Awake and Start method is called
        yield return new WaitForEndOfFrame();
        gameManager.AssignBackButton();
    }

    public void ExitApp() {
        Application.Quit();
    }

}
