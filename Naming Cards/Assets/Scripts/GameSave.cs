﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameSave : MonoBehaviour {

    private string dir;

    void Awake() {
        dir = Application.persistentDataPath;
    }

    public bool SaveDirExists() {
        return Directory.Exists(dir + "/gameSave");
    }

    public bool FileExists(string file) {
        return File.Exists(dir + "/gameSave/" + file);
    }

    public void SaveGame() {

        PriorityQueueJSON jsonQueue = new PriorityQueueJSON();
        jsonQueue.queue = this.gameObject.GetComponent<GameManager>().cardsQueue;
        CardArrayJSON gallery = new CardArrayJSON();
        gallery.array = this.gameObject.GetComponent<GameManager>().knowCards;

        if (!SaveDirExists()) {
            Directory.CreateDirectory(dir + "/gameSave");
        }
        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Create(dir + "/gameSave/queue");
        var json = JsonUtility.ToJson(jsonQueue);
        bf.Serialize(file, json);
        file.Close();

        file = File.Create(dir + "/gameSave/gallery");
        json = JsonUtility.ToJson(gallery);
        bf.Serialize(file, json);
        file.Close();
    }

    public bool LoadGame() {
        if (SaveDirExists()) {
            PriorityQueue cardsQueue = this.gameObject.GetComponent<GameManager>().cardsQueue;
            Card[] knowCards = this.gameObject.GetComponent<GameManager>().knowCards;
            BinaryFormatter bf = new BinaryFormatter();

            if (FileExists("queue")) {
                PriorityQueueJSON temp = new PriorityQueueJSON();
                FileStream file = File.Open(dir + "/gameSave/queue", FileMode.Open);
                JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), temp);
                cardsQueue = temp.queue;
                file.Close();
            }
            else {
                Debug.Log("Error loading queue!");
                return false;
            }

            Debug.Log(cardsQueue.Size());
            knowCards = new Card[cardsQueue.Size()];

            if (FileExists("gallery")) {
                CardArrayJSON temp = new CardArrayJSON();
                FileStream file = File.Open(dir + "/gameSave/gallery", FileMode.Open);
                JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), temp);
                knowCards = temp.array;
                file.Close();
            }
            else {
                Debug.Log("Error loading gallery cards!");   
                return false;
            }

            return true;
        }
        
        return false;
    }

    [Serializable]
    private struct PriorityQueueJSON {
        public PriorityQueue queue;
    }

    [Serializable]
    private struct CardArrayJSON {
        public Card[] array;
    }

}
