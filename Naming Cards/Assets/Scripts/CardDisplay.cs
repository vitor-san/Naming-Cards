using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour {

   	public Card curCard;
   	public Image artwork;
   	public new Text name;
	public bool isToShow;
	private RectTransform rectTrans;
	private Vector3 posWithName;
	private Vector2 originalShape;

	/*
		Awake() method is called when the object is
		instantiated for the first time.
	 */
	void Awake() {
		rectTrans = artwork.GetComponent<RectTransform>();
		posWithName = rectTrans.anchoredPosition;
		originalShape = rectTrans.sizeDelta;
	}
    
	/*
		Method to show the card on screen. It gets
		the attributes from the scriptable object
		attached to this script and arranges them
		on the already existing UI Card element.
	 */
	public void Show() {
		artwork.sprite = curCard.figure;
		artwork.color = Color.white;

        if (isToShow) {
        	name.text = curCard.name.ToLower();
			rectTrans.anchoredPosition = posWithName;	//when is to show, there should be a space for the name to appear
			rectTrans.sizeDelta = originalShape;	//artwork is smaller than when the name doesn't appear
        }
        else {
        	name.text = "";
        	rectTrans.anchoredPosition = Vector3.zero;	//centralizes the artwork
			rectTrans.sizeDelta = new Vector2(originalShape.x * 1.1f, originalShape.y * 1.1f);	//expands it a little bit
        }
    }
	
}
