using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    float arrowSpeed = 10.0f;
    float wheelSpeed = 60.0f;
    float rotateSpeed = 30.0f;

    void Update() {
        // Move Right
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
            transform.Translate(new Vector3(arrowSpeed * Time.deltaTime, 0, 0));
        }

        // Move Left
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
            transform.Translate(new Vector3(-arrowSpeed * Time.deltaTime, 0, 0));
        }

        // Move Down
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
            transform.Translate(new Vector3(0, -arrowSpeed * Time.deltaTime, 0));
        }
        // Move Up
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
            transform.Translate(new Vector3(0, arrowSpeed * Time.deltaTime, 0));
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            transform.Translate(new Vector3(0, 0, wheelSpeed * Time.deltaTime));
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            transform.Translate(new Vector3(0, 0, -wheelSpeed * Time.deltaTime));
        }

        // Rotate Camera left
        if (Input.GetKey(KeyCode.Q)) {
            transform.Rotate(new Vector3(0, 0, rotateSpeed * Time.deltaTime));
        }

        // Rotate Camera right
        if (Input.GetKey(KeyCode.E)) {
            transform.Rotate(new Vector3(0, 0, -rotateSpeed * Time.deltaTime));
        } 
    }
}
