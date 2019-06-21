using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card", order = 51)]
public class Card : ScriptableObject {
	public Sprite figure;
	public new string name;
	public int streak;
	public int delay;
	public bool seen;
	private int[] odds = {1, 3, 5, 7};

	public void Initialize() {
		streak = 0;
		delay = 0;
		seen = false;
	}

	public void CalculateDelay() {
		delay = odds[streak-1];
	}
}
