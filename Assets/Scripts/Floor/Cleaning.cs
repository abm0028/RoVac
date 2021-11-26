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
    int startingPointsBase = 10000;
    int cleaningPoints;
    float floorLevelMultiplier = 1f;

    // Declaration and initialization of the floor tile color values which is a brown color
    Color currentColor = new Color(0.36f, 0.25f, 0.2f);
    bool hasStarted = false;

    int simulationSpeed = 1;

    public TMP_Dropdown speedDropdown, floorDropdown;
    public Button startButton, stopButton;

    // cleaining reduction is the value that is subtracted from the floor tile's dirtiness
    int cleaningReduction;
    // cleanBaseRate is multiplied with the floor multiplier to determine the cleaningReduction above
    int cleanBaseRate = 50;

    // Start is called before the first frame update
    // Will set the color properties of the floor tiles at the start of the program
    void Start() {
        // sets the cleaning points to the starting points 
        cleaningPoints = startingPointsBase;
        // sets the cleaningreduction rate
        cleaningReduction = simulationSpeed * cleanBaseRate;
        // sets the color of the floor tiles to the default color
        gameObject.GetComponent<Renderer>().material.color = currentColor;

        // adds listeners to the start and stop buttons
        startButton.GetComponent<Button>().onClick.AddListener(startAction);
        stopButton.GetComponent<Button>().onClick.AddListener(stopAction);

        // sets listeners to the  for the speed dropdown and floor dropdown
        speedDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate {
            SpeedValueChanged(speedDropdown);
        });
        floorDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate {
            FloorValueChanged(floorDropdown);
        });
    }

    // Will handle the changing of the floor color according to collision with the roVac
    void OnTriggerStay(Collider collision) {

        // if statement to check if the roVac sim has started
        if (hasStarted) {

            // checks to see if cleaning tile touches the vaccum object
            if (collision.gameObject.tag == "Vaccum") {
                // reduces the cleaning points by the math we did below
                if (cleaningPoints > 0)
                    cleaningPoints = cleaningPoints - cleaningReduction;

                // if it gets below zero dirtiness then it will result to 0 for data consistency
                if (cleaningPoints < 0)
                    cleaningPoints = 0;

                // Debug.Log($"Max: {startingPoints} ||| Current points: {cleaningPoints} ||| PCT: %: {getPercentage()}");
                gameObject.GetComponent<Renderer>().material.color = getNewColor();
            }
        }

    }

    // Will be used to find the percentage of the floor that was cleaned
    // called from other classes for data retreival
    public float getPercentage() {
        return Mathf.Abs((float)cleaningPoints / (float)startingPoints);
    }

    // takes the color between two colors depending on the percentage of the floor that was cleaned
    Color getNewColor() {
        return Color.Lerp(Color.white, currentColor, getPercentage());
    }

    // Will change the speed of the simulation when selected by the user
    void switchSimulationSpeed(int choice) {

        switch (choice) {
            case 0:
                cleanBaseRate = 50;
                simulationSpeed = 1;
                break;
            case 1:
                cleanBaseRate = 150;
                simulationSpeed = 50;
                break;
            case 2:
                cleanBaseRate = 300;
                simulationSpeed = 100;
                break;
            default:
                break;
        }

        // changes reduction rate from the similation speed to balance the extra speed
        cleaningReduction = simulationSpeed * cleanBaseRate;
    }

    // handles the multiplier for the floor tiles
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

        // sets the new starting points cleaning values depening on the floor chosen
        startingPoints = (int)(startingPointsBase * floorLevelMultiplier);
        cleaningPoints = startingPoints;
        // changes color of floor depending on the floor chosen
        gameObject.GetComponent<Renderer>().material.color = currentColor;
    }

    // handles the change in floor dropdown 
    void SpeedValueChanged(TMP_Dropdown change) {
        int speedChoice = change.value;
        switchSimulationSpeed(speedChoice);
    }

    // handles the change in floor dropdown 
    void FloorValueChanged(TMP_Dropdown change) {
        switchFloorSettings(change.value);
    }

    void startAction() {
        hasStarted = true;
    }

    // resets the floor tiles to the default color and original valie
    // gets triggered when the stop button is pressed and/or when the similation is over
    // it is called from other classes so we made it public
    public void stopAction() {
        hasStarted = false;
        cleaningPoints = startingPoints;
        gameObject.GetComponent<Renderer>().material.color = currentColor;
    }
}
