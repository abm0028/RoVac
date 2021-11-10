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
    public TMP_Dropdown algorithmDropdown, speedDropdown;
    Rigidbody rb;
    bool allActive = true;
    bool snakingActive, wallfollowActive, spiralActive, randomActive = false;
    int algorithmChoice = 0;
    bool canCollide = true;

    // Declaration and initialization of variables used in simulation and vacuum speed calculation
    float baseSpeed = 10.0f;
    int simulationSpeed = 1;
    float vaccumSpeed;

    int framegoal_50x = 6;
    int incrementStep_50x = 6;

    int framegoal_100x = 3;
    int incrementStep_100x = 3;

    // Declaration and initialization of variables used in the roVac pathing algorithms
    int direction = 1;
    int framecounter = 0;
    int framegoal_1x = 315;
    int incrementStep_1x = 315;

    int turnIndex = 1;
    int turnGoal = 2;

    // Variables specific to the random algorithm
    int numberOfRays = 17;
    public float angle = 90;
    public float rayRange = 0.65f;

    int randomOffset = 30;

    // Start is called before the first frame update
    // Will be used to get the rigid body of the roVac, and handle changing the variable values for simulation speed and pathing algorithm from reading GUI selections 
    void Start() {

        rb = this.GetComponent<Rigidbody>();

        algorithmDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate {
            AlgorithmValueChanged(algorithmDropdown);
        });

        speedDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate {
            SpeedValueChanged(speedDropdown);
        });

        vaccumSpeed = baseSpeed * simulationSpeed;
    }

    void AlgorithmValueChanged(TMP_Dropdown change) {
        algorithmChoice = change.value;
        switchAlgorithims(algorithmChoice);
    }

    void SpeedValueChanged(TMP_Dropdown change) {
        int speedChoice = change.value;
        switchSimulationSpeed(speedChoice);
    }

    void Update() {

    }

    // FixedUpdate is called once per frame
    // Will be used to change the pathing algorithm that will be run
    void FixedUpdate() {

        if (spiralActive) {
            spiralAlgo();
        }

        if (randomActive) {
            randomAlgo();
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
                break;
            case 1:
                simulationSpeed = 50;
                vaccumSpeed = baseSpeed * simulationSpeed;
                break;
            case 2:
                simulationSpeed = 100;
                vaccumSpeed = baseSpeed * simulationSpeed;
                break;
            default:
                simulationSpeed = 1;  // default simulation speed will be set to 1
                vaccumSpeed = baseSpeed * simulationSpeed;
                break;
        }
    }

    // Random Algorithm and instructions for object collision
    void randomAlgo() {
        rb.velocity = transform.forward * Time.fixedDeltaTime * vaccumSpeed;

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, 200) && hitInfo.transform.tag == "Wall") {
            float randRotation = transform.rotation.y;
            transform.Rotate(0, randomTurn(randRotation), 0);
        }
    }

    /* 
    void OnCollisionEnter(Collision collision) {

        if (randomActive) {
            if (collision.gameObject.name.Replace("Prefab(Clone)", "") == "Wall") {
                float randRotation = transform.rotation.y;
                transform.Rotate(0, randomTurn(randRotation), 0);
            }

        }
    }

    */
    
    // Will handle the turning of the roVac when the random algoritm is active
    float randomTurn(float currentRotation) {
        float start = currentRotation + 180;
        int angle = Random.Range(20, 45);
        return start + angle;
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

    /*
    void OnDrawGizmos()
    {
        for (int i = 0; i < numberOfRays; i++)
        {
            var facing = this.transform.rotation;
            var rotationChange = Quaternion.AngleAxis((i / ((float)numberOfRays - 1)) * angle * 2 - angle, this.transform.up);
            var direction = facing * rotationChange * Vector3.forward;
            Gizmos.color = UnityEngine.Color.red;
            Gizmos.DrawRay(this.transform.position, direction);
        }
    }
    */

    // Spiral Algorithm and instructions for object collision and when to spiral
    void spiralAlgo() {
        // transform.position += transform.forward * Time.deltaTime * speed;
        rb.velocity = transform.forward * Time.deltaTime * vaccumSpeed;

        if (simulationSpeed == 1) {
            if (framecounter == framegoal_1x) {
                changeDirections();
                framecounter = 0;
                if (turnIndex == turnGoal) {
                    framegoal_1x += incrementStep_1x;
                    turnIndex = 0;
                }
                turnIndex++;
            }
            framecounter++;
        }

        if (simulationSpeed == 50) {
            if (framecounter == framegoal_50x) {
                changeDirections();
                framecounter = 0;
                if (turnIndex == turnGoal) {
                    framegoal_50x += incrementStep_50x;
                    turnIndex = 0;
                }
                turnIndex++;
            }
            framecounter++;
        }

        if (simulationSpeed == 100) {
            if (framecounter == framegoal_100x) {
                changeDirections();
                framecounter = 0;
                if (turnIndex == turnGoal) {
                    framegoal_100x += incrementStep_100x;
                    turnIndex = 0;
                }
                turnIndex++;
            }
            framecounter++;
        }

        //Debug.Log("counter: " + framecounter + "\n Goal: " + framegoal);
        /* 
        switch (simulationSpeed)
        {
            case 1:
                if (interval == timeInverval)
                {
                    changeDirections();
                    distance++;
                    interval = 0;
                    timeInverval = 75 * distance;
                }
                break;
            case 50:
                if (interval == timeInverval)
                {
                    changeDirections();
                    distance++;
                    interval = 0;
                    timeInverval = 2 * distance;
                }
                break;
        }
        interval++;
        */
    }

    // Will handle changing the direction of the roVac along the cardinal directions for use in the spiral algorithm
    void changeDirections() {

        if (direction == 4) {
            direction = 1;
        }
        else {
            direction++;
        }

        if (direction == 1) {
            Vector3 northDirection = transform.rotation.eulerAngles;
            northDirection.y = 0.0f;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        }

        if (direction == 2) {
            Vector3 eastDirection = transform.rotation.eulerAngles;
            eastDirection.y = 90.0f;
            transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
        }

        if (direction == 3) {
            Vector3 southDirection = transform.rotation.eulerAngles;
            southDirection.y = 180.0f;
            transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        }

        if (direction == 4) {
            Vector3 westDirection = transform.rotation.eulerAngles;
            westDirection.y = 270.0f;
            transform.rotation = Quaternion.Euler(0.0f, 270.0f, 0.0f);
        }
    }

    // Will run all algorithms if none are specified 
    void allAlgo() {
        Debug.Log("all");
    }

}