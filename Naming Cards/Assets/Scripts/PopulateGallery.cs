﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateGallery : MonoBehaviour {
    
    public GameObject cardPrefab;
    private GameManager gameManager;
    private SceneChange sceneChange;
    
    void Awake() {
        GameObject obj = GameObject.Find("SceneChange");
		sceneChange = obj.GetComponent<SceneChange>();
        obj = GameObject.Find("GameManager");
        gameManager = obj.GetComponent<GameManager>();
        
        Populate();
    }

    private void Populate() {
        GameObject newCard; //instantiates card
        CardDisplay cardUI;

        foreach (Card c in gameManager.knowCards) {
            newCard = (GameObject)Instantiate(cardPrefab, transform);
            cardUI = newCard.GetComponent<CardDisplay>();

            if (c != null) {
                cardUI.curCard = c;
			    cardUI.isToShow = true;
			    cardUI.Show();
            }
            else {
                Debug.Log("Oi");
            }
        }
    }

}