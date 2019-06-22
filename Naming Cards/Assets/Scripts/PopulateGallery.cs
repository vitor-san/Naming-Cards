using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateGallery : MonoBehaviour {
    
    public GameObject cardPrefab;
    private GameManager gameManager;
    private SceneChange sceneChange;
    public Sprite locked;
    
    /*
		Awake() method is called when the object is
		instantiated for the first time.
	 */
    void Awake() {
        GameObject obj = GameObject.Find("SceneChange");
		sceneChange = obj.GetComponent<SceneChange>();
        obj = GameObject.Find("GameManager");
        gameManager = obj.GetComponent<GameManager>();
        
        Populate();
    }

    /*
        Fills the Grid Layout of the Scroll View with the cards
        in Game Manager's "knowCards" array. If the card doesn't
        exist, it shows a "locked" card figure, instead.
     */
    private void Populate() {
        GameObject newCard;
        RectTransform cardTransf;
        CardDisplay cardUI;
        Image cardBack, cardFront;

        foreach (Card c in gameManager.knowCards) {
            newCard = (GameObject)Instantiate(cardPrefab, transform);   //instantiate a new card, that will be child of "Content" object
            cardTransf = newCard.GetComponent<RectTransform>();
            cardTransf.localScale = new Vector3(0.5875f, 0.55f, 1f);    //reescales the card to fit on Grid Layout's predefined slot size
            cardUI = newCard.GetComponent<CardDisplay>();

            if (c != null) {    //show the dominated card
                cardUI.curCard = c;
			    cardUI.isToShow = true;
			    cardUI.Show();
            }
            else {  //show the "locked" card figure
                cardBack = newCard.transform.GetChild(0).gameObject.GetComponent<Image>();
                cardBack.sprite = locked;
                cardFront = newCard.transform.GetChild(1).gameObject.GetComponent<Image>();
                cardFront.sprite = null;
            }
        }
    }

}
