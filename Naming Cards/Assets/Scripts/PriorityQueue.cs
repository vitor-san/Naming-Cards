using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue {
    List<Card> list = new List<Card>();
    
    public void Enqueue(Card c) {
        list.Add(c);
    }
    
    public Card Dequeue() {
        int min_delay = 30000;
        for (int i = 0; i < list.Count; i++) {
            min_delay = list[i].delay < min_delay ? list[i].delay : min_delay;
        }
        for (int i = 0; i < list.Count; i++) {
            if (list[i].delay == min_delay) {
                Card c = list[i];
                list.RemoveAt(i);
                return c;
            }
        }
        return null;
    }
    
    public void NextRound() {
        for (int i = 0; i < list.Count; i++) {
            if (list[i].delay > 0) list[i].delay--;
        }
    }
}
