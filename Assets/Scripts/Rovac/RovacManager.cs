/*
 * Class:          RovacManager
 * Purpose:        This algorithm will handle all of the functionality of the roVac, which are instructions for each pathing algorthm, and changing the simulation speed.
 * Authors:        Edson Jaramillo, Alec Mueller, Samuel Strong     
 * Notes:          
 * Date Created:   11/02/21
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RovacManager : MonoBehaviour {

    public TMP_Dropdown algorithmDropdown, speedDropdown, floorDropdown;
    public Button startButton;
    public TMP_Text batteryText;
    Rigidbody rb;
    bool allActive = true;
    bool snakingActive, wallfollowActive, spiralActive, randomActive = false;
    int algorithmChoice = 0;
    bool hasStarted = false;
    int timeFrameCounter = 0;
    int timeGoal = 450000;
    int frameInterval;

    // Declaration and initialization of variables used in simulation and vacuum speed calculation
    float baseSpeed = 10.0f;
    int simulationSpeed = 1;
    float vaccumSpeed;

    int framegoal_1x = 315;
    int incrementStep_1x = 315;

    int framegoal_50x = 6;
    int incrementStep_50x = 6;

    int framegoal_100x = 3;
    int incrementStep_100x = 3;

    // Declaration and initialization of variables used in the roVac pathing algorithms


    // variables used for spiral algorithms
    int framecounter = 0;

    int turnIndex = 1;
    int turnGoal = 2;

    // Variables specific to the random algorithm



    // Start is called before the first frame update
    // Will be used to get the rigid body of the roVac, and handle changing the variable values for simulation speed and pathing algorithm from reading GUI selections 
    void Start() {

        rb = this.GetComponent<Rigidbody>();

        startButton.GetComponent<Button>().onClick.AddListener(startAction);

        algorithmDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate {
            AlgorithmValueChanged(algorithmDropdown);
        });

        speedDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate {
            SpeedValueChanged(speedDropdown);
        });

        vaccumSpeed = baseSpeed * simulationSpeed;
        frameInterval = 1 * simulationSpeed;


    }

    // Update is called once per frame
    // Will be used to check the raycast collisions when the random algoritm is active
    void Update() {
        if (hasStarted) {
            /* Raycast for random algorithm */
            if (randomActive) {
                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hitInfo;

                if (Physics.Raycast(ray, out hitInfo, 250) && hitInfo.transform.tag == "Wall") {
                    float randRotation = transform.rotation.y;
                    transform.Rotate(0, randomTurn(randRotation), 0);
                }
            }
        }
    }

    // FixedUpdate is called once per frame
    // Will be used to change the pathing algorithm that will be run
    void FixedUpdate() {

        if (hasStarted) {
            if (spiralActive) {
                spiralAlgo();
            }

            if (randomActive) {
                randomAlgo();
            }

            timeManager();
        }
        
    }

    void timeManager() {
        framecounter = framecounter + frameInterval;
        batteryText.text = $"Battery Remaining: {getMinutes(framecounter)} minutes";
        framecounter++;
    }

    string getMinutes(int frames) {
        float seconds = frames / 50;
        float minutes = (int)seconds / 60;

        return $"{150 - (int)minutes}";
    }

    void AlgorithmValueChanged(TMP_Dropdown change) {
        algorithmChoice = change.value;
        switchAlgorithims(algorithmChoice);
    }

    void SpeedValueChanged(TMP_Dropdown change) {
        int speedChoice = change.value;
        switchSimulationSpeed(speedChoice);
    }

    // Will reset all algorithm bools to prepare for changing the algorithm
    void resetActive() {
        allActive = false;
        snakingActive = false;
        wallfollowActive = false;
        spiralActive = false;
        randomActive = false;
    }

    // Will read the integer value denoting the seleted algorithm and changed it base on that
    void switchAlgorithims(int choice) {

        switch (choice) {
            case 0:
                resetActive();
                allActive = true;
                break;
            case 1:
                resetActive();
                snakingActive = true;
                break;
            case 2:
                resetActive();
                wallfollowActive = true;
                break;
            case 3:
                resetActive();
                spiralActive = true;
                break;
            case 4:
                resetActive();
                randomActive = true;
                break;
            default:
                //all algorithms run
                break;
        }
    }

    // Will change the vacuum speed based on the simulation speed selected
    void switchSimulationSpeed(int choice) {

        switch (choice) {
            case 0:
                simulationSpeed = 1;
                vaccumSpeed = baseSpeed * simulationSpeed;
                frameInterval = 1 * simulationSpeed;
                break;
            case 1:
                simulationSpeed = 50;
                vaccumSpeed = baseSpeed * simulationSpeed;
                frameInterval = 1 * simulationSpeed;
                break;
            case 2:
                simulationSpeed = 100;
                vaccumSpeed = baseSpeed * simulationSpeed;
                frameInterval = 1 * simulationSpeed;
                break;
            default:
                break;
        }
    }

    // Random Algorithm and instructions for object collision
    void randomAlgo() {
        rb.velocity = transform.forward * Time.fixedDeltaTime * vaccumSpeed;
    }

    // Will handle the turning of the roVac when the random algoritm is active
    float randomTurn(float currentRotation) {

        float start = currentRotation + 180;
        int angle = Random.Range(20, 45);
        int choice = Random.Range(1, 3);

        if (choice == 1) {
            return start + angle;
        }
        else {
            return start - angle;
        }
    }

    // Changes the angle of trajectory of the roVac after collision with an object based on unit circle calculations
    float normalizeDegree(float degree) {

        if (degree > 360) {
            return degree - 360;
        }
        else {
            return degree;
        }
    }

    // Spiral Algorithm and instructions for object collision and sensing when to spiral
    void spiralAlgo() {

        rb.velocity = transform.forward * Time.deltaTime * vaccumSpeed;

        switch (simulationSpeed) {
            case 1:
                spiralSpeedManager(ref framegoal_1x, ref incrementStep_1x);
                break;
            case 50:
                spiralSpeedManager(ref framegoal_50x, ref incrementStep_50x);
                break;
            case 100:
                spiralSpeedManager(ref framegoal_100x, ref incrementStep_100x);
                break;
            default:
                break;
        }
    }

    // manages the speed and intervals of the spirals
    void spiralSpeedManager(ref int goal, ref int incrementStep) {

        if (framecounter == goal) {
            transform.Rotate(0, 90, 00);
            framecounter = 0;
            if (turnIndex == turnGoal) {
                goal += incrementStep;
                turnIndex = 0;
            }
            turnIndex++;
        }
        framecounter++;
    }

    // Will run all algorithms if none are specified 
    void allAlgo() {
        Debug.Log("all");
    }

    void startAction() {
        hasStarted = true;
    }

}