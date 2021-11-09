using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleaning : MonoBehaviour {
    int startingPoints = 1000;
    int cleaningPoints;
    int r = 92;
    int g = 64;
    int b = 51;

    Color floorColor;

    // Start is called before the first frame update
    void Start() {
        cleaningPoints = startingPoints;
        floorColor = new Color(r / 255f, g / 255f, b / 255f);
        gameObject.GetComponent<Renderer>().material.color = floorColor;
    }

    // Update is called once per frame
    void Update() {

    }

    void OnTriggerStay(Collider collision) {

        if (collision.gameObject.tag == "Vaccum") {
            if (cleaningPoints > 0)
                cleaningPoints = cleaningPoints - 25;
            gameObject.GetComponent<Renderer>().material.color = getNewColor();
        }

    }

    public float getPercentage() {
        return Mathf.Abs((float)cleaningPoints / (float)startingPoints);
    }

    Color getNewColor() {
        float percentage = getPercentage();
        return Color.Lerp(Color.white, floorColor, percentage);
    }
}
