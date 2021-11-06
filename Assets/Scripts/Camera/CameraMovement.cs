using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    float arrowSpeed = 10.0f;
    float wheelSpeed = 60.0f;
    float rotateSpeed = 30.0f;

    void Update() {
        if (Input.GetKey(KeyCode.RightArrow)) {
            transform.Translate(new Vector3(arrowSpeed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.LeftArrow)) {
            transform.Translate(new Vector3(-arrowSpeed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            transform.Translate(new Vector3(0, -arrowSpeed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.UpArrow)) {
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

        if (Input.GetKey(KeyCode.A)) {
            transform.Rotate(new Vector3(0, 0, rotateSpeed * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.D)) {
            transform.Rotate(new Vector3(0, 0, -rotateSpeed * Time.deltaTime));
        }

    }
}
