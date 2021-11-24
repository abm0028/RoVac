using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class DataDisplay : MonoBehaviour {

    string path = @"Assets/Resources/records.csv";
    public TMP_Text template;
    public GameObject parentUI;
    int startingY = 65;
    int xPosition = 20;
    int counter = 0;

    Stack records = new Stack();

    // Start is called before the first frame update
    void Start() {
        readFile();
        displayRecords();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyUp(KeyCode.C)) {
            readFile();
            Debug.Log("Read file");
        }

    }

    void readFile() {

        using (StreamReader sr = File.OpenText(path)) {
            while (!sr.EndOfStream) {
                string line = sr.ReadLine();
                if (!line.Contains("Date")) {
                    records.Push(line);
                }

            }
            sr.Close();
        }

        foreach (string record in records) {
            Debug.Log(record);
        }

    }

    void displayRecords() {

        foreach (string record in records) {
            if (counter < 8) {

                string[] datapoints = record.Split(',');
                displayTextValue(20, datapoints[0]);
                displayTextValue(190, datapoints[1]);
                displayTextValue(280, datapoints[2]);
                displayTextValue(420, datapoints[3]);
                displayTextValue(580, datapoints[4]);
                displayTextValue(685, datapoints[5]);
                startingY -= 30;
            }
            counter++;
        }

    }

    void displayTextValue(float xPos, string text) {
        Vector3 positon = new Vector3(xPos, startingY, 0);
        TMP_Text newTextBox = Instantiate(template, positon, Quaternion.identity, parentUI.transform);
        newTextBox.GetComponent<RectTransform>().localPosition = positon;
        newTextBox.text = text;
    }

}
