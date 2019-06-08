using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameManager : MonoBehaviour {

	private Queue<Card> cardsQueue;
	private int numCardsThisRound = 5;
	private const int maxNumCards = 10;

    public void StartGame() {
    	this.cardsQueue = new Queue<Card>();
    }

	public void PlayGame() {
		SceneManager.LoadScene("MainScene");
		this.StartGame();
	}
}
