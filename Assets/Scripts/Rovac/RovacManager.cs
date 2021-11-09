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
    bool canCollider = true;

    float baseSpeed = 10.0f;
    int simulationSpeed = 1;
    float vaccumSpeed;

    //spiral algo variables
    int direction = 1;
    int framecounter = 0;
    int framegoal_1x = 315;
    int incrementStep_1x = 315;

    int framegoal_50x = 6;
    int incrementStep_50x = 6;

    int framegoal_100x = 3;
    int incrementStep_100x = 3;

    int turnIndex = 1;
    int turnGoal = 2;

    //random algo variables
    int numberOfRays = 17;
    public float angle = 90;
    public float rayRange = 0.65f;

    int randomOffset = 30;




    // Start is called before the first frame update
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

    // FixedUpdate is called once per frame
    void FixedUpdate() {

        if (spiralActive) {
            spiralAlgo();
        }

        if (randomActive) {
            randomAlgo();
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

    void resetActive() {
        allActive = false;
        snakingActive = false;
        wallfollowActive = false;
        spiralActive = false;
        randomActive = false;
    }

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
                break;
        }
    }

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
                break;
        }
    }

    // Random Algorithm and its functions
    void randomAlgo() {
        rb.velocity = transform.forward * Time.deltaTime * vaccumSpeed;
        // rb.MovePosition = transform.forward * Time.deltaTime;
    }

    
    void OnCollisionEnter(Collision collision) {

        if (randomActive) {
            if (collision.gameObject.name.Replace("Prefab(Clone)", "") == "Wall") {
                float randRotation = transform.rotation.y;
                transform.Rotate(0, randomTurn(randRotation), 0);
            }

        }
    }
   
    float randomTurn(float currentRotation) {
        // float start = Mathf.Abs(currentRotation);
        float start = currentRotation + 180;

        int choice = Random.Range(1, 3);
        int angle = Random.Range(35, 46);
        if (choice == 1) {
            return start + 13;
        }
        else {
            return start - 13;
        }
    }

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


    // Spiral algo and its functions
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


    // all algos
    void allAlgo() {
        Debug.Log("all");
    }

}