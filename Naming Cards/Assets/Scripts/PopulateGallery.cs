using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateGallery : MonoBehaviour {
    
    public GameObject cardPrefab;
    private GameManager gameManager;
    private SceneChange sceneChange;
    public Sprite locked;
    
    void Awake() {
        GameObject obj = GameObject.Find("SceneChange");
		sceneChange = obj.GetComponent<SceneChange>();
        obj = GameObject.Find("GameManager");
        gameManager = obj.GetComponent<GameManager>();
        
        Populate();
    }

    private void Populate() {
        GameObject newCard;
        RectTransform cardTransf;
        CardDisplay cardUI;
        Image cardBack, cardFront;

        foreach (Card c in gameManager.knowCards) {
            newCard = (GameObject)Instantiate(cardPrefab, transform);
            cardTransf = newCard.GetComponent<RectTransform>();
            cardTransf.localScale = new Vector3(0.5875f, 0.55f, 1f);
            cardUI = newCard.GetComponent<CardDisplay>();

            if (c != null) {
                cardUI.curCard = c;
			    cardUI.isToShow = true;
			    cardUI.Show();
            }
            else {
                cardBack = newCard.transform.GetChild(0).gameObject.GetComponent<Image>();
                cardBack.sprite = locked;
                cardFront = newCard.transform.GetChild(1).gameObject.GetComponent<Image>();
                cardFront.sprite = null;
            }
        }
    }

}
