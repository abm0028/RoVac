/*
 * Class:          RovacManager
 * Purpose:        This algorithm will handle all of the functionality of the roVac, which are instructions for each pathing algorthm, and changing the simulation speed.
 * Authors:        Edson Jaramillo, Alec Mueller, Samuel Strong, Marshall Wright
 * Notes:          
 * Date Created:   11/02/21
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;
using UnityEngine.UI;

public class RovacManager : MonoBehaviour {

    // GUI Elements
    public TMP_Dropdown algorithmDropdown, speedDropdown, floorDropdown;
    public TMP_InputField IDField;
    public Button startButton, stopButton;
    public GameObject panel;
    public TMP_Text batteryText, cleaningText;

    // Rovac variables
    Vector3 rovacPosition;
    Rigidbody rb;

    public Camera cameraobj;

    // algorthim variables
    bool allActive, snakingActive = true;
    bool wallfollowActive, spiralActive, randomActive = false;
    int algorithmChoice = 0;
    String selectedAlgorithm = "All";
    bool hasStarted = false;

    // time management variables
    int timeFrameCounter = 0;
    int timeGoal = 450000;
    int frameInterval;
    bool cooldown = false;
    int cooldownCounter = 0;
    int cooldownGoal = 3000;

    // Declaration and initialization of variables used in simulation and vacuum speed calculation
    float baseSpeed = 10.0f;
    int simulationSpeed = 1;
    float vaccumSpeed;
    float raycastLength = 1f;

    // variables used for timing for spiral algorithm
    int framegoal_1xStartingPoint = 315;
    int framegoal_1x = 315;
    int incrementStep_1x = 315;

    int framegoal_50xStartingPoint = 6;
    int framegoal_50x = 6;
    int incrementStep_50x = 6;

    int framegoal_100xStartingPoint = 3;
    int framegoal_100x = 3;
    int incrementStep_100x = 3;

    // Variables used for snaking algorithm
    int frameSnakingCounter = 0;

    // Variables used for spiral algorithm
    int frameSpiralCounter = 0;

    int turnIndex = 1;
    int turnGoal = 2;

    // path for saving data
    string path;

    // variable for run ID
    string IDName = "ID";

    // sets path
    string setPath(string path) {
        if (Application.isEditor) {
            return $@"Assets/Resources/{path}";
        } else {
            return $"{Application.dataPath}/StreamingAssets/{path}";
        }
    }

    // Start is called before the first frame update
    // Will be used to get the rigid body of the roVac, and handle changing the variable values for simulation speed and pathing algorithm from reading GUI selections 
    void Start() {

        path = setPath("records.csv");
        rovacPosition = transform.position;

        rb = this.GetComponent<Rigidbody>();

        startButton.GetComponent<Button>().onClick.AddListener(startAction);
        stopButton.GetComponent<Button>().onClick.AddListener(stopAction);

        algorithmDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate {
            AlgorithmValueChanged(algorithmDropdown);
        });

        speedDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate {
            SpeedValueChanged(speedDropdown);
        });

        //IDField = GameObject.Find("InputField (TMP)").GetComponent<TMP_InputField>();
        IDField.GetComponent<TMP_InputField>().onValueChanged.AddListener(inputAction);

        vaccumSpeed = baseSpeed * simulationSpeed;
        frameInterval = 1 * simulationSpeed;
        allActive = true;
    }

    // Update is called once per frame
    // Will be used to check the raycast collisions when the random algorithm is active
    void Update() {
        if (hasStarted) {
            // Raycast for random algorithm
            if (randomActive) {
                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hitInfo;
                // rotates the rovac a random directions
                if (Physics.Raycast(ray, out hitInfo, raycastLength) && hitInfo.transform.tag == "Wall") {
                    float randRotation = transform.rotation.y;
                    transform.Rotate(0, randomTurn(randRotation), 0);
                }
            }

            if (spiralActive) {
                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hitInfo;
                // spirals the rovac
                if (Physics.Raycast(ray, out hitInfo, raycastLength) && hitInfo.transform.tag == "Wall") {
                    float randRotation = transform.rotation.y;
                    transform.Rotate(0, randomTurn(randRotation), 0);
                    frameSpiralCounter = 0;
                    resetSpiralTimers();
                    cooldown = true;
                }
            }

            if (snakingActive) {
                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, raycastLength) && (hitInfo.transform.tag == "Wall" || hitInfo.transform.tag == "Chest")) {
                    float currentFacing = transform.rotation.y;
                    transform.Rotate(0, snakingTurn(currentFacing), 0);
                }
            }

            if (wallfollowActive) {

                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hitInfo;
                float angle = UnityEngine.Random.Range(-1, 1) * 45;

                if (Physics.Raycast(ray, out hitInfo, raycastLength) && (hitInfo.transform.tag == "Wall" || hitInfo.transform.tag == "Chest")) {
                    transform.Rotate(0, angle, 0);
                }
            }
        }
    }

    // FixedUpdate is called once per frame
    // Will be used to change the algorithm that will be run
    void FixedUpdate() {

        if (hasStarted) {
            if (spiralActive) {
                if (cooldown) {
                    randomAlgo();
                    if (cooldownCounter == cooldownGoal) {
                        cooldown = false;
                        cooldownCounter = 0;
                    }
                    cooldownCounter++;
                } else {
                    spiralAlgo();
                }
            }

            if (randomActive) {
                randomAlgo();
            }

            if (snakingActive) {
                snakingAlgo();
            }

            if (wallfollowActive) {
                wallfollowAlgo();
            }

            // manages the time updates clean text percentage
            timeManager();
            updateCleanText();
        }
    }

    // updates clean percentage text
    void updateCleanText() {
        float cleanPct = cameraobj.GetComponent<ObjectPlacement>().getAverages();
        cleaningText.text = $"Cleaning: {convertToPercentage(cleanPct)}";
    }

    // Will reset all algorithm bools to prepare for changing the algorithm
    void resetActive() {
        snakingActive = false;
        wallfollowActive = false;
        spiralActive = false;
        randomActive = false;
    }

    /*------------------------------------------ Random Algo ------------------------------------------*/

    // Random Algorithm and instructions for object collision
    void randomAlgo() {
        rb.velocity = transform.forward * Time.fixedDeltaTime * vaccumSpeed;
    }

    // Will handle the turning of the roVac when the random algorithm is active
    float randomTurn(float currentRotation) {

        float start = currentRotation + 180;
        int angle = UnityEngine.Random.Range(20, 45);
        int choice = UnityEngine.Random.Range(1, 3);

        if (choice == 1) {
            return start + angle;
        } else {
            return start - angle;
        }
    }

    // Changes the angle of trajectory of the roVac after collision with an object based on unit circle calculations
    float normalizeDegree(float degree) {

        if (degree > 360) {
            return degree - 360;
        } else {
            return degree;
        }
    }

    /*------------------------------------------ Snaking Algo -----------------------------------------*/

    void snakingAlgo() {
        rb.velocity = transform.forward * Time.deltaTime * vaccumSpeed;
    }

    // snaking algorithm turn manager
    float snakingTurn(float currentRotation) {
        return currentRotation + 190;
    }

    /*---------------------------------------- Wall-Follow Algo ---------------------------------------*/

    void wallfollowAlgo() {
        rb.velocity = transform.forward * Time.fixedDeltaTime * vaccumSpeed;
    }

    /*------------------------------------------ Sprial Algo ------------------------------------------*/

    // Manages the speed and intervals of the spirals
    void spiralSpeedManager(ref int goal, ref int incrementStep) {
        if (frameSpiralCounter == goal) {
            transform.Rotate(0, 90, 00);
            frameSpiralCounter = 0;
            if (turnIndex == turnGoal) {
                goal += incrementStep;
                turnIndex = 0;
            }
            turnIndex++;
        }
        frameSpiralCounter++;
    }

    // Manages the spiral algorithm turn
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

    /*------------------------------------------ Time managment ------------------------------------------*/

    // manages the time and resets time after each algo completes
    void timeManager() {
        timeFrameCounter = timeFrameCounter + frameInterval;
        batteryText.text = $"Battery Remaining: {getMinutes(timeFrameCounter)} minutes";
        if (timeFrameCounter >= timeGoal) {
            panel.GetComponent<UIManager>().stopAction();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            finishAction();
            if (allActive) {
                resetFloorsAllAlgo();
            } else {
                resetFloors();
            }
        }
    }

    // gets minutes from seconds
    string getMinutes(int frames) {
        float seconds = frames / 50;
        float minutes = (int)seconds / 60;
        return $"{150 - (int)minutes}";
    }

    // resets the floors
    void resetFloors() {
        List<GameObject> tempFloor = cameraobj.GetComponent<ObjectPlacement>().floorCollection;
        foreach (GameObject floor in tempFloor) {
            floor.GetComponent<Cleaning>().stopAction();
        }
    }

    // resets the floors for all algorithm
    // difference is is starts listening for the rovac immediately after resetting the floors
    void resetFloorsAllAlgo() {
        List<GameObject> tempFloor = cameraobj.GetComponent<ObjectPlacement>().floorCollection;
        foreach (GameObject floor in tempFloor) {
            floor.GetComponent<Cleaning>().stopActionAllAlgo();
        }
    }

    // spiral reset
    void resetSpiralTimers() {
        framegoal_1x = framegoal_1xStartingPoint;
        framegoal_50x = framegoal_50xStartingPoint;
        framegoal_100x = framegoal_100xStartingPoint;
    }

    // snaking reset
    void resetSnakingTimers() {
        framegoal_1x = framegoal_1xStartingPoint;
        framegoal_50x = framegoal_50xStartingPoint;
        framegoal_100x = framegoal_100xStartingPoint;
    }

    /*----------------------------------------- Data Recording -----------------------------------------*/

    // records data to csv
    void recordData() {
        createFile();
        appendToFile();
    }

    // creates the file new file if does not exist
    void createFile() {

        if (File.Exists(path)) {
            return;
        } else {
            using (StreamWriter sw = File.CreateText(path)) {
                sw.Close();
            }

            using (StreamWriter sw = File.AppendText(path)) {
                String output = $"Date, Algorithm, Algorithm Version, ID, Cleaning Pct, Time remaining";
                sw.WriteLine(output);
                sw.Close();
            }
            return;
        }
    }

    // Convert float to percentage
    string convertToPercentage(float value) {
        double rounded = Math.Round(value, 2);
        return $"{rounded * 100}%";
    }

    // appends run to the next line of the records.csv file
    void appendToFile() {
        using (StreamWriter sw = File.AppendText(path)) {
            String humanDate = DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss");
            String minutes = getMinutes(timeFrameCounter);
            float average = cameraobj.GetComponent<ObjectPlacement>().getAverages();

            String output = $"{humanDate}, {selectedAlgorithm}, {selectedAlgorithm} 1.0,{IDName}, {convertToPercentage(average)}, {minutes} minutes";
            sw.WriteLine(output);
            sw.Close();
        }
    }

    // handles input from run ID input field
    void inputAction(string value) {
        // To get the text
        IDName = value;
    }

    /*---------------------------------------------- Buttons ----------------------------------------------*/
    // starts the simulation
    void startAction() {
        int floorcount = cameraobj.GetComponent<ObjectPlacement>().getFloorCount();
        if (floorcount >= 200 && floorcount <= 8000) {
            hasStarted = true;
        }
    }

    // runs on finish of simulation
    void finishAction() {
        panel.GetComponent<UIManager>().showUI();
        hasStarted = false;
        transform.position = rovacPosition;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        batteryText.text = $"Battery Remaining: {getMinutes(timeFrameCounter)} minutes";
        cleaningText.text = $"Cleaning: 0%";
        if (allActive == false) {
            timeFrameCounter = 0;
            recordData();
        }

        if (allActive == true) {
            if (snakingActive == true) {
                selectedAlgorithm = "Snaking";
                recordData();
                timeFrameCounter = 0;
                resetActive();
                wallfollowActive = true;
                startAction();
                panel.GetComponent<UIManager>().hideUI();
            } else if (wallfollowActive == true) {
                selectedAlgorithm = "Wall-follow";
                recordData();
                timeFrameCounter = 0;
                resetActive();
                spiralActive = true;
                startAction();
                panel.GetComponent<UIManager>().hideUI();
            } else if (spiralActive == true) {
                selectedAlgorithm = "Spiral";
                recordData();
                timeFrameCounter = 0;
                resetActive();
                randomActive = true;
                startAction();
                panel.GetComponent<UIManager>().hideUI();
            } else if (randomActive == true) {
                selectedAlgorithm = "Random";
                recordData();
                timeFrameCounter = 0;
                resetActive();
            }
        }
    }

    // runs if user mannually stops the simulation
    void stopAction() {
        panel.GetComponent<UIManager>().showUI();
        recordData();
        hasStarted = false;
        transform.position = rovacPosition;
        transform.rotation = Quaternion.identity;
        timeFrameCounter = 0;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        batteryText.text = $"Battery Remaining: {getMinutes(timeFrameCounter)} minutes";
        cleaningText.text = $"Cleaning: 0%";
    }

    // Will read the integer value denoting the seleted algorithm and changed it base on that
    void switchAlgorithims(int choice) {
        switch (choice) {
            case 0:
                resetActive();
                selectedAlgorithm = "All";
                allActive = true;
                snakingActive = true;
                break;
            case 1:
                resetActive();
                selectedAlgorithm = "Snaking";
                allActive = false;
                snakingActive = true;
                break;
            case 2:
                resetActive();
                selectedAlgorithm = "Wall-follow";
                allActive = false;
                wallfollowActive = true;
                break;
            case 3:
                resetActive();
                selectedAlgorithm = "Spiral";
                allActive = false;
                spiralActive = true;
                break;
            case 4:
                resetActive();
                selectedAlgorithm = "Random";
                allActive = false;
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
                cooldownGoal = 1500;
                break;
            case 1:
                simulationSpeed = 50;
                vaccumSpeed = baseSpeed * simulationSpeed;
                frameInterval = 1 * simulationSpeed;
                cooldownGoal = 150;
                break;
            case 2:
                simulationSpeed = 100;
                vaccumSpeed = baseSpeed * simulationSpeed;
                frameInterval = 1 * simulationSpeed;
                cooldownGoal = 75;
                break;
            default:
                break;
        }
    }

    // handles algorithm selection change
    void AlgorithmValueChanged(TMP_Dropdown change) {
        algorithmChoice = change.value;
        switchAlgorithims(algorithmChoice);
    }

    // handles speed selection change
    void SpeedValueChanged(TMP_Dropdown change) {
        int speedChoice = change.value;
        switchSimulationSpeed(speedChoice);
    }
}