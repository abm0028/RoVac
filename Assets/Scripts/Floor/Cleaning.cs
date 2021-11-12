/*
 * Class:          Cleaning
 * Purpose:        This algorithm will handle the cleaning of the floor tiles, and display its "dirtiness" changing from interactions with the roVac.
 * Authors:        Edson Jaramillo, Alec Mueller, Samuel Strong     
 * Notes:          
 * Date Created:   11/08/21
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Cleaning : MonoBehaviour {

    // Declaration and initialization of color values for the floor tiles, and roVac cleaning speed
    int startingPoints = 10000;
    int cleaningPoints;
    float floorLevelMultiplier = 1f;

    Color currentColor;
    bool hasStarted = false;

    int simulationSpeed = 1;

    public TMP_Dropdown speedDropdown, floorDropdown;
    public Button startButton;

    int cleaningReduction;
    int cleanBaseRate = 250;

    // Start is called before the first frame update
    // Will set the color properties of the floor tiles at the start of the program
    void Start() {
        cleaningPoints = startingPoints;
        cleaningReduction = simulationSpeed * cleanBaseRate;
        currentColor = new Color(0.36f, 0.25f, 0.2f);
        gameObject.GetComponent<Renderer>().material.color = currentColor;
        startButton.GetComponent<Button>().onClick.AddListener(startAction);
        speedDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate {
            SpeedValueChanged(speedDropdown);
        });
        floorDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate {
            FloorValueChanged(floorDropdown);
        });
    }

    // Update is called once per frame
    void Update() {

    }

    // Will handle the changing of the floor color according to collision with the roVac
    void OnTriggerStay(Collider collision) {

        if (hasStarted) {

            if (collision.gameObject.tag == "Vaccum") {
                if (cleaningPoints > 0)
                    cleaningPoints = cleaningPoints - cleaningReduction;

                if (cleaningPoints < 0)
                    cleaningPoints = 0;

                //Debug.Log($"Total: {startingPoints} ||| Cleaning points: {cleaningPoints} ||| PCT: %: {getPercentage()}");
                gameObject.GetComponent<Renderer>().material.color = getNewColor();
            }
        }

    }

    // Will be used to find the percentage of the floor that was cleaned
    public float getPercentage() {
        return Mathf.Abs((float)cleaningPoints / (float)startingPoints);
    }

    Color getNewColor() {
        return Color.Lerp(Color.white, currentColor, getPercentage());
    }

    // Will change the speed of the simulation when selected by the user
    void switchSimulationSpeed(int choice) {

        switch (choice) {
            case 0:
                simulationSpeed = 1;
                break;
            case 1:
                simulationSpeed = 50;
                break;
            case 2:
                simulationSpeed = 100;
                break;
            default:
                break;
        }
        cleaningReduction = simulationSpeed * cleanBaseRate;
    }

    void switchFloorSettings(int choice) {
        switch (choice) {
            case 0:
                floorLevelMultiplier = 1f;
                currentColor = new Color(0.36f, 0.25f, 0.2f);
                break;
            case 1:
                floorLevelMultiplier = 2f;
                currentColor = new Color(0f, 0.39f, 0f);
                break;
            case 2:
                floorLevelMultiplier = 2.5f;
                currentColor = new Color(1f, 0.4f, 0f);
                break;
            case 3:
                floorLevelMultiplier = 3f;
                currentColor = new Color(1f, 0f, 0f);
                break;
            default:
                break;
        }

        startingPoints = (int)(startingPoints * floorLevelMultiplier);
        cleaningPoints = startingPoints;
        gameObject.GetComponent<Renderer>().material.color = currentColor;
    }

    // Will implement changes to the speed of the simulation when selected by the user
    void SpeedValueChanged(TMP_Dropdown change) {
        int speedChoice = change.value;
        switchSimulationSpeed(speedChoice);
    }

    void FloorValueChanged(TMP_Dropdown change) {
        switchFloorSettings(change.value);
    }

    void startAction() {
        hasStarted = true;
    }
}
