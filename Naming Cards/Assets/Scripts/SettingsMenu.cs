using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour {
    
    public AudioMixer audioMixer;

    private GameManager gameManager;
    private SceneChange sceneChange;

    void Awake() {
        GameObject obj = GameObject.Find("SceneChange");
		sceneChange = obj.GetComponent<SceneChange>();
        obj = GameObject.Find("GameManager");
        gameManager = obj.GetComponent<GameManager>();
    }

    public void SetVolume(float volume) {
        audioMixer.SetFloat("volume", volume);
    }

    public void setAnswerDifficulty(int difficultyIndex) {
        if (difficultyIndex == 0) gameManager.answerDelay = 6;
        if (difficultyIndex == 1) gameManager.answerDelay = 3;
        if (difficultyIndex == 2) gameManager.answerDelay = 2;
        if (difficultyIndex == 3) gameManager.answerDelay = 1;
    }

    public void setLearnDifficulty(int difficultyIndex) {
        if (difficultyIndex == 0) gameManager.learnDelay = 6;
        if (difficultyIndex == 1) gameManager.learnDelay = 3;
        if (difficultyIndex == 2) gameManager.learnDelay = 2;
        if (difficultyIndex == 3) gameManager.learnDelay = 1;
    }
}
