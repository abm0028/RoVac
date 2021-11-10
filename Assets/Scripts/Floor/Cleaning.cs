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
    int startingPoints = 1000;
    int cleaningPoints;
    int r = 92;
    int g = 64;
    int b = 51;
    Color floorColor;

    int simulationSpeed = 1;

    public TMP_Dropdown speedDropdown;
    int cleaningReduction;
    int cleanBaseRate = 5;

    // Start is called before the first frame update
    // Will set the color properties of the floor tiles at the start of the program
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

    // Will handle the changing of the floor color according to collision with the roVac
    void OnTriggerStay(Collider collision) {

        if (collision.gameObject.tag == "Vaccum") {
            if (cleaningPoints > 0)
                cleaningPoints = cleaningPoints - cleaningReduction;

            if(cleaningPoints < 0)
                cleaningPoints = 0;
           
            gameObject.GetComponent<Renderer>().material.color = getNewColor();
        }

    }

    // Will be used to find the percentage of the floor that was cleaned
    public float getPercentage() {
        return Mathf.Abs((float)cleaningPoints / (float)startingPoints);
    }

    Color getNewColor() {
        float percentage = getPercentage();
        return Color.Lerp(Color.white, floorColor, percentage);
    }

    // Will change the speed of the simulation when selected by the user
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
                simulationSpeed = 1;  // will run at a simulation speed of 1 by default
                cleaningReduction = simulationSpeed * cleanBaseRate;
                break;
        }
    }

    // Will implement changes to the speed of the simulation when selected by the user
    void SpeedValueChanged(TMP_Dropdown change) {
        int speedChoice = change.value;
        switchSimulationSpeed(speedChoice);
    }
}
