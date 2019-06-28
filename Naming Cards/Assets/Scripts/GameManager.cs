using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class GameManager : MonoBehaviour {

	private GameObject showCounter, guessCounter, inputFieldObj, readyGo, roundEnd;
	private SceneChange sceneChange;
	private GameSave gameSaver;
	private Text iterator, endTitle, endScore;
	private Slider showTimer, guessTimer;
	private InputField inputField;
	private Button goButton, stopButton, continueButton, backButton;
	private CardDisplay cardUI;
	public PriorityQueue cardsQueue;
	public Card[] knowCards;
	private int knowCardsCount = 0;
	private const int maxNumCards = 10;
	[SerializeField] private int numCardsThisRound = 5;	//"Serialize Field" makes Unity show the attribute in the inspector, even if it's private
	public float learnDelay = 2.5f;
	public float answerDelay = 5f;
	private string answer = "";

	/*
		Awake() method is called when the object is
		instantiated for the first time.
	 */
	private void Awake() {
		DontDestroyOnLoad(this.gameObject);	//the game object in which this script is attached to (i.e. the Game Manager itself) will persist between scenes (i.e. only one instance of it will be created, which will help to control the game batter)
		GameObject sceneChangeObj = GameObject.Find("SceneChange");
		sceneChange = sceneChangeObj.GetComponent<SceneChange>();
		gameSaver = this.gameObject.GetComponent<GameSave>();
		cardsQueue = new PriorityQueue();

		if (true || !gameSaver.LoadGame()) {	//if the game is being runned for the first time (here, the first condition of the if statement was set to not call the second one, because it's not done yet and would cause the game to crash)
			Card[] cards = Resources.LoadAll("Cards", typeof(Card)).Cast<Card>().ToArray();	//load all cards from "Resources" folder
			Shuffle(cards);
			//put all cards in the queue
			foreach (Card c in cards) {
				c.Initialize();		//initializes the card streak, delay and seen with 0
				cardsQueue.Enqueue(c);
			}

			knowCards = new Card[cardsQueue.Size()];	//instantiates the array of know cards (which will start empty)	
		}
	}

	/*
		Finds "BackButton" on current scene and adds to
		it the method to go back to the main menu when
		pressed.
	 */
	public void AssignBackButton() {
		GameObject obj = GameObject.Find("BackButton");
		backButton = obj.GetComponent<Button>();
		backButton.onClick.RemoveAllListeners();
		backButton.onClick.AddListener(delegate { 
			sceneChange.MainMenu(); 
		});
	}

	/*
		Assigns all references of objects/components
		in the scene to their respective representatives 
		on this class attributes.
	 */
	private void AssignAllReferences() {
		GameObject obj;	//a generic game object
		
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

	/*
		Method to be called when the GameManager 
		enters the MainScene scene.
	 */
    public void StartGame() {
		AssignAllReferences();

		inputFieldObj.SetActive(false);
		guessCounter.SetActive(false);
		roundEnd.SetActive(false);
    	Time.timeScale = 0f;	//first, the user have to press the button "Bora!", and this line of code will impede the game to keep running on background until that happens
    }

	/*
		When the player press the "Bora!" button, the game 
		will come out from it's frozen state and time will 
		start playing normally.
	 */
	private void UnlockGame() {
		Time.timeScale = 1f;
		readyGo.SetActive(false);
		StartCoroutine(NewShowRound(numCardsThisRound));
	}

	/*
		Coroutine that shows a specific number of cards to
		the player, giving to him a time to memorize their
		names.

		Parameter:
			int numCards - number of cards to be shown this
			round
	 */
	private IEnumerator NewShowRound(int numCards) {
		Card[] curGame = new Card[numCards];	//instantiates a new array of cards
		for (int i = 0; i < numCards; i++) curGame[i] = cardsQueue.Dequeue();	//pops from queue the number of cards to be used in this round

		int numShowed = 0;
		foreach (Card c in curGame) {
			if (c == null) break;
			numShowed++;
			iterator.text = numShowed + "/" + numCardsThisRound;	//updates the counter of how many cards have been shown this round
			cardUI.curCard = c;

			float delay;
			if (!c.seen) {
				cardUI.isToShow = true;
				delay = learnDelay;
			}
			else {
				cardUI.isToShow = false;
				delay = learnDelay/2;
			}

			cardUI.Show();
			showTimer.SetValueWithoutNotify(1);	//fill slider's time counter to max
			StartCoroutine(CountTime(delay, showTimer));
			yield return new WaitForSeconds(delay + 0.5f);	//waits for a time (the time to deplete slider's time counter) before going to the next card
		}

		showCounter.SetActive(false);
		iterator.enabled = false;
		guessCounter.SetActive(true);
		inputFieldObj.SetActive(true);
		StartCoroutine(NewGuessRound(curGame));
	}

	/*
		Coroutine that starts the guessing round, in which
		the player will try to remember and hit the name of
		the cards that were shown to him in the learn round.

		Parameter:
			Card[] curGame - the game formed by previous show
			round
	 */
	private IEnumerator NewGuessRound(Card[] curGame) {
		int correctGuesses = 0;
		Shuffle(curGame);

		foreach (Card c in curGame) {
			cardUI.curCard = c;
			cardUI.isToShow = false;
			cardUI.Show();
			inputField.text = "";

			//asks the answer
			guessTimer.SetValueWithoutNotify(1);	//fill slider's time counter to max
			StartCoroutine(CountTime(answerDelay, guessTimer));
			yield return new WaitForSeconds(answerDelay + 0.5f);	//waits for a time (the time to deplete slider's time counter) before proceeding

			//validates answer
			if (answer == c.name.ToLower()) {
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

			if (c.streak == 1) knowCards[knowCardsCount++] = c;	//if the player has guessed the card x times in a row, he dominated it, so it goes to his gallery instead of back in the queue (sorta like a trophy)
			else cardsQueue.Enqueue(c);
			
			answer = "";
		}

		if (numCardsThisRound < maxNumCards) numCardsThisRound++;
		ResultScreen(correctGuesses);
	}

	/*
		Coroutine that depletes a slider's time counter. 

		Parameters:
			float initialTime - total time of the counter
			Slider slider - slider to be modified
	 */
	private IEnumerator CountTime(float initialTime, Slider slider) {
		float timer = initialTime;
		while (timer >= 0) {
			slider.SetValueWithoutNotify(timer/initialTime);	//sets time counter to a percent of the initial time
			yield return null;
			timer -= Time.deltaTime;	//decreases time
		}
	}

	/*
		Shuffles an array of Cards.

		Parameter:
			Card[] cards - array to be
			shuffled
	 */
	private void Shuffle(Card[] cards) {
		Card temp;
		for (int i = 0; i < cards.Length; i++) {
            int rand = Random.Range(0, cards.Length);
            temp = cards[rand];
            cards[rand] = cards[i];
            cards[i] = temp;
        }
	}

	/*
		Method to be used as the event triggered when 
		the Input Field has an answer submitted to it.

		Parameter:
			string input - input got from the user
	 */
	private void SetAnswer(string input) {
		answer = input.ToLower();
	}

	/*
		Show the final result of the round and asks
		the player if he wants to continue or not.

		Parameter:
			int numHits - number of cards guessed
			correctly by the end of this round
	 */
	private void ResultScreen(int numHits) {
		inputFieldObj.SetActive(false);
		guessCounter.SetActive(false);
		roundEnd.SetActive(true);

		if (numHits >= Mathf.Ceil(numCardsThisRound/2f)) {	//if the player hit more than a half of the cards...
			endTitle.text = "Parabéns!";
			if (answerDelay > learnDelay) answerDelay -= 0.3f;	//decreases answer time (improving game dificulty)
		}
		else {
			endTitle.text = "Que pena!";
			answerDelay = learnDelay*2;
		}

		if (numHits == 1) endScore.text = "Você acertou 1 carta";
		else endScore.text = "Você acertou " + numHits + " cartas";
	}

	/*
		Method to begin a new round.
	 */
	private void Continue() {
		showCounter.SetActive(true);
		iterator.enabled = true;
		roundEnd.SetActive(false);
		cardsQueue.NextRound();
		StartCoroutine(NewShowRound(numCardsThisRound));
	}
}
