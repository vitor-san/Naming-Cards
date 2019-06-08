using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject {
	public Sprite figure;
	public new string name;
	public int streak = 0;
	public int delay = 0;
}
