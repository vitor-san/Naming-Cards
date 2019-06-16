using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class GameManager : MonoBehaviour {

	private GameObject showCounter, guessCounter, inputFieldObj, readyGo, roundEnd;
	private SceneChange sceneChange;
	private Text iterator, endTitle, endScore;
	private Slider showTimer, guessTimer;
	private InputField inputField;
	private Button goButton, stopButton, continueButton;
	private CardDisplay cardUI;
	private PriorityQueue cardsQueue;
	private const int maxNumCards = 10;
	[SerializeField] private int numCardsThisRound = 5;
	[SerializeField] private float delayAmount = 2.5f;	//for now, will be constant
	private string answer = "";

	private void Awake() {
		DontDestroyOnLoad(this.gameObject);
		GameObject sceneChangeObj = GameObject.Find("SceneChange");
		sceneChange = sceneChangeObj.GetComponent<SceneChange>();
	}

	private void AssignAllReferences() {
		GameObject obj;	// a generic game object
		//assign each respective reference to all game objects/components of this class
		obj = GameObject.Find("Iterator");
		iterator = obj.GetComponent<Text>();

		obj = GameObject.Find("ShowingTimeCounter/Slider");
		showTimer = obj.GetComponent<Slider>();

		obj = GameObject.Find("GuessingTimeCounter/Slider");
		guessTimer = obj.GetComponent<Slider>();

		obj = GameObject.Find("Card");
		cardUI = obj.GetComponent<CardDisplay>();

		roundEnd = GameObject.Find("RoundEnd");
		endTitle = GameObject.Find("Title").GetComponent<Text>();
		endScore = GameObject.Find("Score").GetComponent<Text>();

		obj = GameObject.Find("ReadyPanel/Button");
		goButton = obj.GetComponent<Button>();
		goButton.onClick.RemoveAllListeners();
		goButton.onClick.AddListener(delegate { 
			UnlockGame(); 
		});

		obj = GameObject.Find("ButtonPanel/StopButton");
		stopButton = obj.GetComponent<Button>();
		stopButton.onClick.RemoveAllListeners();
		stopButton.onClick.AddListener(delegate { 
			sceneChange.MainMenu(); 
		});

		obj = GameObject.Find("ButtonPanel/ContinueButton");
		continueButton = obj.GetComponent<Button>();
		continueButton.onClick.RemoveAllListeners();
		continueButton.onClick.AddListener(delegate { 
			Continue();
		});

		inputFieldObj = GameObject.Find("InputField");
		inputField = inputFieldObj.GetComponent<InputField>();
		inputField.onEndEdit.RemoveAllListeners();
		inputField.onEndEdit.AddListener(delegate { 
			SetAnswer(inputField.text); 
		});

		showCounter = GameObject.Find("ShowingTimeCounter");
		guessCounter = GameObject.Find("GuessingTimeCounter");
		readyGo = GameObject.Find("DarkerPanel");
	}

    public void StartGame() {
		AssignAllReferences();
    	cardsQueue = new PriorityQueue();
		
		//put all cards in a queue
		Card[] cards = Resources.LoadAll("Cards", typeof(Card)).Cast<Card>().ToArray();
		foreach (Card c in cards) {
			c.Initialize();		//initializes the card streak, delay and seen with 0
			cardsQueue.Enqueue(c);
		}

		inputFieldObj.SetActive(false);
		guessCounter.SetActive(false);
		roundEnd.SetActive(false);
    	Time.timeScale = 0f;
    }

	private void UnlockGame() {
		Time.timeScale = 1f;
		readyGo.SetActive(false);
		StartCoroutine(NewShowRound(numCardsThisRound));
	}

	private IEnumerator NewShowRound(int numCards) {
		Card[] curGame = new Card[numCards];
		for (int i = 0; i < numCards; i++) curGame[i] = cardsQueue.Dequeue();

		int numShowed = 0;
		foreach (Card c in curGame) {
			numShowed++;
			iterator.text = numShowed + "/" + numCardsThisRound;
			cardUI.curCard = c;
			if (!c.seen) cardUI.isToShow = true;
			else cardUI.isToShow = false;
			cardUI.Show();
			showTimer.SetValueWithoutNotify(1);
			StartCoroutine(CountTime(delayAmount, showTimer));
			yield return new WaitForSeconds(delayAmount + 1);
		}

		showCounter.SetActive(false);
		iterator.enabled = false;
		guessCounter.SetActive(true);
		inputFieldObj.SetActive(true);
		StartCoroutine(NewGuessRound(curGame));
	}

	private IEnumerator NewGuessRound(Card[] curGame) {
		int correctGuesses = 0;

		//TODO: embaralhar

		foreach (Card c in curGame) {
			cardUI.curCard = c;
			cardUI.isToShow = false;
			cardUI.Show();
			inputField.text = "";
			//perguntar a resposta
			guessTimer.SetValueWithoutNotify(1);
			StartCoroutine(CountTime(delayAmount*2, guessTimer));
			yield return new WaitForSeconds(delayAmount*2 + 1);
			if (answer == c.name.ToLower()) {	//faz a comparacao para ver se o jogador acertou
				correctGuesses++;
				c.streak++;
				c.seen = true;
				c.CalculateDelay();
			}
			else {
				c.streak = 0;
				c.delay = 0;
				c.seen = false;
			}
			answer = "";
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

	private void SetAnswer(string input) {
		answer = input.ToLower();
	}

	private void ResultScreen(int numHits) {
		inputFieldObj.SetActive(false);
		guessCounter.SetActive(false);
		roundEnd.SetActive(true);

		if (numHits >= Mathf.Ceil(numCardsThisRound/2f)) endTitle.text = "Parabéns!";
		else endTitle.text = "Que pena!";

		if (numHits == 1) endScore.text = "Você acertou 1 carta";
		else endScore.text = "Você acertou " + numHits + " cartas";
	}

	private void Continue() {
		showCounter.SetActive(true);
		iterator.enabled = true;
		roundEnd.SetActive(false);
		cardsQueue.NextRound();
		StartCoroutine(NewShowRound(numCardsThisRound));
	}
}
