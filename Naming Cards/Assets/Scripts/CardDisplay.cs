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

	void Awake() {
		rectTrans = artwork.GetComponent<RectTransform>();
		posWithName = rectTrans.anchoredPosition;
		originalShape = rectTrans.sizeDelta;
	}
    
	public void Show() {
		artwork.sprite = curCard.figure;
		artwork.color = Color.white;
        if (isToShow) {
        	name.text = curCard.name.ToLower();
			rectTrans.anchoredPosition = posWithName;
			rectTrans.sizeDelta = originalShape;
        }
        else {
        	name.text = "";
        	rectTrans.anchoredPosition = Vector3.zero;
			rectTrans.sizeDelta = new Vector2(originalShape.x * 1.1f, originalShape.y * 1.1f);
        }
    }
	
}
