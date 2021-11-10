using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Cleaning : MonoBehaviour {
    int startingPoints = 1000;
    int cleaningPoints;
    int r = 92;
    int g = 64;
    int b = 51;

    int simulationSpeed = 1;

    Color floorColor;

    public TMP_Dropdown speedDropdown;
    int cleaningReduction;
    int cleanBaseRate = 5;

    // Start is called before the first frame update
    void Start() {
        cleaningPoints = startingPoints;
        cleaningReduction = simulationSpeed * cleanBaseRate;
        floorColor = new Color(r / 255f, g / 255f, b / 255f);
        gameObject.GetComponent<Renderer>().material.color = floorColor;
        speedDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate {
            SpeedValueChanged(speedDropdown);
        });
    }

    // Update is called once per frame
    void Update() {

    }

    void OnTriggerStay(Collider collision) {

        if (collision.gameObject.tag == "Vaccum") {
            if (cleaningPoints > 0)
                cleaningPoints = cleaningPoints - cleaningReduction;

            if(cleaningPoints < 0)
                cleaningPoints = 0;
           
            gameObject.GetComponent<Renderer>().material.color = getNewColor();
        }

    }

    public float getPercentage() {
        return Mathf.Abs((float)cleaningPoints / (float)startingPoints);
    }

    Color getNewColor() {
        float percentage = getPercentage();
        return Color.Lerp(Color.white, floorColor, percentage);
    }

    void switchSimulationSpeed(int choice) {
        switch (choice) {
            case 0:
                simulationSpeed = 1;
                cleaningReduction = simulationSpeed * cleanBaseRate;
                break;
            case 1:
                simulationSpeed = 50;
                cleaningReduction = simulationSpeed * cleanBaseRate;
                break;
            case 2:
                simulationSpeed = 100;
                cleaningReduction = simulationSpeed * cleanBaseRate;
                break;
            default:
                break;
        }
    }

    void SpeedValueChanged(TMP_Dropdown change) {
        int speedChoice = change.value;
        switchSimulationSpeed(speedChoice);
    }
}
