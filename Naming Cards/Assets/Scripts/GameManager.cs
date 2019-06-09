using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameManager : MonoBehaviour {

	public GameObject cardUI;
	private Queue<Card> cardsQueue;
	private int numCardsThisRound = 1;	//5
	[SerializeField] private double showTime = 2.5;	//for now, will be constant
	private int numCardsShowed;
	private const int maxNumCards = 2;	//10

    public void StartGame() {
    	cardsQueue = new Queue<Card>();
    	//puts all cards in the queue
		Card[] cards = Resources.LoadAll("Cards", typeof(Card)).Cast<Card>().ToArray();
		foreach (Card c in cards) cardsQueue.Enqueue(c);
    	newShowRound(numCardsThisRound);
    }

	public void PlayGame() {
		SceneManager.LoadScene("MainScene");
		StartGame();
	}

	private void newShowRound(int numCards) {
		Card[] curGame = new Card[numCards];
		for (int i = 0; i < numCards; i++) curGame[i] = cardsQueue.Dequeue();
		foreach (Card c in curGame) {
			
		}
	}

	private void newGuessRound(int numCards) {

		if (numCardsThisRound < maxNumCards) numCardsThisRound++;
	}
}
