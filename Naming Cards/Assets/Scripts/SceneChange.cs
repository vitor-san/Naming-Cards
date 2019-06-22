using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour {
    
    private GameManager gameManager;
    private GameSave gameSave;

    /*
		Awake() method is called when the object is
		instantiated for the first time.
	 */
    void Awake() {
		DontDestroyOnLoad(this.gameObject); //the object in which this script is attached will persist between scenes
        GameObject gameManagerObj = GameObject.Find("GameManager");
        gameManager = gameManagerObj.GetComponent<GameManager>();
        gameSave = gameManagerObj.GetComponent<GameSave>();
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
        //start loading the scene
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);
        //wait until the level finish loading
        while (!asyncLoadLevel.isDone) yield return null;
        //wait a frame so every Awake and Start method is called
        yield return new WaitForEndOfFrame();
        gameManager.StartGame();
    }

    private IEnumerator LoadGallery() {
        //start loading the scene
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync("GalleryScene", LoadSceneMode.Single);
        //wait until the level finish loading
        while (!asyncLoadLevel.isDone) yield return null;
        //wait a frame so every Awake and Start method is called
        yield return new WaitForEndOfFrame();
        gameManager.AssignBackButton();
    }

    public void ExitApp() {
        gameSave.SaveGame();
        Application.Quit();
    }

}
