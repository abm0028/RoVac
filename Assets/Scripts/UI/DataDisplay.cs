using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class DataDisplay : MonoBehaviour {

    string path;
    public TMP_Text template;
    public GameObject parentUI;
    int startingY = 115;
    int counter = 0;
    public Button refreshButton;
    List<GameObject> textRecords = new List<GameObject>();

    Stack records = new Stack();

    string setPath(string path) {

        if (Application.isEditor) {

            return $@"Assets/Resources/{path}";
        } else {
            return $"{Application.dataPath}/StreamingAssets/{path}";
        }
    }

    // Start is called before the first frame update
    void Start() {
        path = setPath("records.csv");
        readFile();
        displayRecords();
        refreshButton.onClick.AddListener(refreshAction);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyUp(KeyCode.C)) {
            readFile();
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
    }

    void displayRecords() {
        foreach (string record in records) {
            if (counter < 12) {
                string[] datapoints = record.Split(',');
                displayTextValue(20, datapoints[0]);
                displayTextValue(190, datapoints[1]);
                displayTextValue(280, datapoints[2]);
                displayTextValue(420, datapoints[3]);
                displayTextValue(580, datapoints[4]);
                displayTextValue(685, datapoints[5]);
                startingY -= 24;
            }
            counter++;
        }
        counter = 0;
        startingY = 115;
    }

    void displayTextValue(float xPos, string text) {
        Vector3 positon = new Vector3(xPos, startingY, 0);
        TMP_Text newTextBox = Instantiate(template, positon, Quaternion.identity, parentUI.transform);
        newTextBox.GetComponent<RectTransform>().localPosition = positon;
        newTextBox.text = text;
        string percentageText;
        if (newTextBox.text.Contains("%")) {
            percentageText = newTextBox.text.Replace("%", "");
            float value = float.Parse(percentageText);
            if (value < 50) {
                newTextBox.color = Color.red;
            } else if (value >= 51 && value <= 75) {
                newTextBox.color = Color.yellow;
            } else {
                newTextBox.color = Color.green;
            }
        }
        textRecords.Add(newTextBox.gameObject);
    }

    void refreshAction() {
        deleteRecordsFromScreen();
        readFile();
        displayRecords();
    }

    void deleteRecordsFromScreen() {
        foreach (GameObject text in textRecords) {
            Destroy(text);
        }
        textRecords.Clear();
        records.Clear();
    }
}
