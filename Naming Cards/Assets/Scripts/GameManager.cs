using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameManager : MonoBehaviour {

	public GameObject cardUI, showCounter, guessCounter;
	public Text iterator;
	public Slider showTimer, guessTimer;
	public InputField inputField;
	private CardDisplay cardUIScript;
	private Queue<Card> cardsQueue;
	
	private const int maxNumCards = 2;	//10
	[SerializeField] private int numCardsThisRound = 2;	//5
	[SerializeField] private float delayAmount = 2.5f;	//for now, will be constant

	void Awake() {
		DontDestroyOnLoad(this.gameObject);
	}

	public void PlayGame() {
		SceneManager.LoadScene("MainScene");
		StartGame();
	}

    public void StartGame() {
		GameObject botao = GameObject.Find("Button");
		botao.SetActive(false);	//retirar essas duas depois.

		cardUIScript = cardUI.GetComponent<CardDisplay>();
    	cardsQueue = new Queue<Card>();
    	//puts all cards in the queue
		Card[] cards = Resources.LoadAll("Cards", typeof(Card)).Cast<Card>().ToArray();
		foreach (Card c in cards) cardsQueue.Enqueue(c);
    	StartCoroutine(NewShowRound(numCardsThisRound));
    }

	private IEnumerator NewShowRound(int numCards) {
		Card[] curGame = new Card[numCards];
		for (int i = 0; i < numCards; i++) curGame[i] = cardsQueue.Dequeue();

		int numShowed = 0;
		foreach (Card c in curGame) {
			numShowed++;
			iterator.text = numShowed + "/" + numCardsThisRound;
			cardUIScript.curCard = c;
			cardUIScript.isToShow = true;
			cardUIScript.Show();
			showTimer.SetValueWithoutNotify(1);
			StartCoroutine(CountTime(delayAmount, showTimer));
			yield return new WaitForSeconds(delayAmount + 1);
		}

		showCounter.SetActive(false);
		iterator.enabled = false;
		guessCounter.SetActive(true);
		inputField.ActivateInputField();
		StartCoroutine(NewGuessRound(curGame));
	}

	private IEnumerator NewGuessRound(Card[] curGame) {
		int correctGuesses = 0;
		//embaralhar
		foreach (Card c in curGame) {
			cardUIScript.curCard = c;
			cardUIScript.isToShow = false;
			cardUIScript.Show();
			//perguntar a resposta
			guessTimer.SetValueWithoutNotify(1);
			StartCoroutine(CountTime(delayAmount*2, guessTimer));
			string resposta = GetAnswer();
			yield return new WaitForSeconds(delayAmount*2 + 1);
			if (resposta.ToLower() == c.name.ToLower()) {	//faz a comparacao para ver se o jogador acertou
				correctGuesses++;
				c.streak++;
			}
			cardsQueue.Enqueue(c);
		}

		if (numCardsThisRound < maxNumCards) numCardsThisRound++;
		ResultScreen(correctGuesses);
	}

	private IEnumerator CountTime(float initialTime, Slider slider) {
		float timer = initialTime;
		while (timer >= 0) {
			slider.SetValueWithoutNotify(timer/initialTime);
			yield return null;
			timer -= Time.deltaTime;
		}
	}

	public string GetAnswer() {
		return "wolf";
	}

	private void ResultScreen(int numHits) {
		Debug.Log("You have guessed " + numHits + " cards correctly!");
		showCounter.SetActive(true);
		iterator.enabled = true;
		guessCounter.SetActive(false);
		inputField.DeactivateInputField();	//not working
		StartCoroutine(NewShowRound(numCardsThisRound));
	}
}
