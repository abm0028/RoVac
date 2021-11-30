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
    int startingY = 65;
    int xPosition = 20;
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
            if (counter < 6) {

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

        counter = 0;
        startingY = 65;

    }

    void displayTextValue(float xPos, string text) {
        Vector3 positon = new Vector3(xPos, startingY, 0);
        TMP_Text newTextBox = Instantiate(template, positon, Quaternion.identity, parentUI.transform);
        newTextBox.GetComponent<RectTransform>().localPosition = positon;
        newTextBox.text = text;
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
