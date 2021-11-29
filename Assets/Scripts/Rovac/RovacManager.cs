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
using System;
using System.IO;
using UnityEngine.UI;

public class RovacManager : MonoBehaviour {

    public TMP_Dropdown algorithmDropdown, speedDropdown, floorDropdown;
    public TMP_InputField IDField;
    public Button startButton, stopButton;
    public Camera cameraobj;
    public GameObject panel;

    Vector3 rovacPosition;

    ObjectPlacement objectscript;
    GUIContent content;

    public TMP_Text batteryText;
    Rigidbody rb;
    bool allActive = true;
    bool snakingActive, wallfollowActive, spiralActive, randomActive = false;
    int algorithmChoice = 0;
    String selectedAlgorithm = "All";
    bool hasStarted = false;
    int timeFrameCounter = 0;
    int timeGoalStartingPoint = 450000;
    int timeGoal = 450000;
    // int timeGoal = 10000;
    int frameInterval;
    bool cooldown = false;
    int cooldownCounter = 0;
    int cooldownGoal = 1500;

    // Declaration and initialization of variables used in simulation and vacuum speed calculation
    float baseSpeed = 10.0f;
    int simulationSpeed = 1;
    float vaccumSpeed;

    int framegoal_1xStartingPoint = 315;
    int framegoal_1x = 315;
    int incrementStep_1x = 315;

    int framegoal_50xStartingPoint = 6;
    int framegoal_50x = 6;
    int incrementStep_50x = 6;

    int framegoal_100xStartingPoint = 3;
    int framegoal_100x = 3;
    int incrementStep_100x = 3;

    float raycastLength = 1f;


    // Declaration and initialization of variables used in the roVac pathing algorithms

    // Variables used for snaking algorithm
    int frameSnakingCounter = 0;

    // Variables used for spiral algorithm
    int frameSpiralCounter = 0;

    int turnIndex = 1;

    int turnGoal = 2;

    string path;

    string IDName = "ID";

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
    }




    // Update is called once per frame
    // Will be used to check the raycast collisions when the random algoritm is active
    void Update() {
        if (hasStarted) {
            // Raycast for random algorithm
            if (randomActive) {
                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hitInfo;

                if (Physics.Raycast(ray, out hitInfo, raycastLength) && hitInfo.transform.tag == "Wall") {
                    float randRotation = transform.rotation.y;
                    transform.Rotate(0, randomTurn(randRotation), 0);
                }
            }

            if (spiralActive) {
                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hitInfo;

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
                    Debug.Log("i hit a wall");
                    transform.Rotate(0, 90, 0);
                    frameSnakingCounter = 0;
                    resetSnakingTimers();
                }
            }
        
            if (wallfollowActive) {
                
                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hitInfo;
                float angle = UnityEngine.Random.Range(-1, 1)*45;

                if (Physics.Raycast(ray, out hitInfo, raycastLength) && (hitInfo.transform.tag == "Wall" || hitInfo.transform.tag == "Chest")) {
                    float randRotation = transform.rotation.y;
                    transform.Rotate(0, angle, 0);
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.P)) {
            recordData();
        }
    }

    // FixedUpdate is called once per frame
    // Will be used to change the pathing algorithm that will be run
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

            if(snakingActive){
                snakingAlgo();
            }

            if(wallfollowActive){
                wallfollowAlgo();
            }

            timeManager();
        }
    }

    // Will reset all algorithm bools to prepare for changing the algorithm
    void resetActive() {
        allActive = false;
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

    // Will handle the turning of the roVac when the random algoritm is active
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
                    
        switch (simulationSpeed) {
            case 1:
                snakingTurnManager(ref framegoal_1x);
                break;
            case 50:
                snakingTurnManager(ref framegoal_50x);
                break;
            case 100:
                snakingTurnManager(ref framegoal_100x);
                break;
            default:
                break;
            }
    }

    void snakingTurnManager(ref int goal) {
        
        if (frameSnakingCounter == goal) {
            transform.Rotate(0, 90, 0);
            Debug.Log("turning back");
            frameSnakingCounter = 0;
        }
        else {
            frameSnakingCounter++;
        }
    }

    /*---------------------------------------- Wall-Follow Algo ---------------------------------------*/

    void wallfollowAlgo()
    {
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

    /*------------------------------------------ All algo ------------------------------------------*/

    // Will run all algorithms if none are specified 
    void allAlgo() {
        Debug.Log("all");
    }

    /*------------------------------------------ Time managment ------------------------------------------*/

    void timeManager() {
        timeFrameCounter = timeFrameCounter + frameInterval;
        batteryText.text = $"Battery Remaining: {getMinutes(timeFrameCounter)} minutes";
        if (timeFrameCounter >= timeGoal) {
            panel.GetComponent<UIManager>().stopAction();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            stopAction();
            resetFloors();
        }
    }

    string getMinutes(int frames) {
        float seconds = frames / 50;
        float minutes = (int)seconds / 60;

        return $"{150 - (int)minutes}";
    }

    void resetFloors() {

        List<GameObject> tempFloor = cameraobj.GetComponent<ObjectPlacement>().floorCollection;

        foreach (GameObject floor in tempFloor) {
            floor.GetComponent<Cleaning>().stopAction();
        }

    }

    void resetSpiralTimers() {
        framegoal_1x = framegoal_1xStartingPoint;
        framegoal_50x = framegoal_50xStartingPoint;
        framegoal_100x = framegoal_100xStartingPoint;
    }

    void resetSnakingTimers() {
        framegoal_1x = framegoal_1xStartingPoint;
        framegoal_50x = framegoal_50xStartingPoint;
        framegoal_100x = framegoal_100xStartingPoint;
    }

    /*----------------------------------------- Data Recording -----------------------------------------*/

    void recordData() {

        createFile();
        appendToFile();
    }

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

    // convert float to percentage
    string convertToPercentage(float value) {
        double rounded = Math.Round(value, 2);
        return $"{rounded * 100}%";
    }

    void appendToFile() {
        using (StreamWriter sw = File.AppendText(path)) {

            String pcDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
            String humanDate = DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss");
            String minutes = getMinutes(timeFrameCounter);
            float average = cameraobj.GetComponent<ObjectPlacement>().getAverages();

            String output = $"{humanDate}, {selectedAlgorithm}, {selectedAlgorithm} 1.0,{IDName}, {convertToPercentage(average)}, {minutes} minutes";
            sw.WriteLine(output);
            sw.Close();
        }
    }

    void inputAction(string value) {
        // To get the text
        IDName = value;

    }

    /*---------------------------------------------- Buttons ----------------------------------------------*/
    void startAction() {
        int floorcount = cameraobj.GetComponent<ObjectPlacement>().getFloorCount();
        if (floorcount >= 200 && floorcount <= 8000) {
            hasStarted = true;
        } else {

        }
    }

    void stopAction() {
        recordData();
        hasStarted = false;
        transform.position = rovacPosition;
        timeFrameCounter = 0;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        batteryText.text = $"Battery Remaining: {getMinutes(timeFrameCounter)} minutes";
    }



    // Will read the integer value denoting the seleted algorithm and changed it base on that
    void switchAlgorithims(int choice) {

        switch (choice) {
            case 0:
                resetActive();
                selectedAlgorithm = "All";
                allActive = true;
                break;
            case 1:
                resetActive();
                selectedAlgorithm = "Snaking";
                snakingActive = true;
                break;
            case 2:
                resetActive();
                selectedAlgorithm = "Wall-follow";
                wallfollowActive = true;
                break;
            case 3:
                resetActive();
                selectedAlgorithm = "Spiral";
                spiralActive = true;
                break;
            case 4:
                resetActive();
                selectedAlgorithm = "Random";
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

    void AlgorithmValueChanged(TMP_Dropdown change) {
        algorithmChoice = change.value;
        switchAlgorithims(algorithmChoice);
    }

    void SpeedValueChanged(TMP_Dropdown change) {
        int speedChoice = change.value;
        switchSimulationSpeed(speedChoice);
    }
}