using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class ObjectPlacement : MonoBehaviour {

    public GameObject Chest, Wall, Floor;
    public GameObject ChestMouse, WallMouse, FloorMouse;
    private bool chestActive = false;
    private bool wallActive = true;
    private bool floorActive = false;
    private bool deleteActive = true;
    int rotationDirection = 1;
    Quaternion rotationAngle = Quaternion.Euler(0.0f, 0.0f, 0.0f);
    List<GameObject> chestCollection = new List<GameObject>();
    List<GameObject> wallCollection = new List<GameObject>();
    List<GameObject> floorCollection = new List<GameObject>();
    Cleaning cleaningobject;

    Vector3 worldPoint = Vector3.zero;

    public Button wallButton, floorButton, chestButton, saveButton, loadButton, tableButton;

     string path = @"default.txt";
    //string path = @"test.txt";

    // Start is called before the first frame update
    void Start() {
        worldPoint = getWorldPoint();
        wallButton.GetComponent<Button>().onClick.AddListener(wallAction);
        floorButton.GetComponent<Button>().onClick.AddListener(floorAction);
        chestButton.GetComponent<Button>().onClick.AddListener(chestAction);

        saveButton.GetComponent<Button>().onClick.AddListener(saveAction);
        loadButton.GetComponent<Button>().onClick.AddListener(loadAction);
    }

    void rotateObjects() {
        if (rotationDirection == 4) {
            rotationDirection = 1;
        }
        else {
            rotationDirection++;
        }

        switch (rotationDirection) {
            case 1:
                rotationAngle = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                break;
            case 2:
                rotationAngle = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                break;
            case 3:
                rotationAngle = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                break;
            case 4:
                rotationAngle = Quaternion.Euler(0.0f, 270.0f, 0.0f);
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update() {
        placeObject();
        objectKeyboardListener();
    }

    void placeObject() {
        if (wallActive) {
            worldPoint = getWorldPoint();
            if (worldPoint != Vector3.zero) {
                WallMouse.transform.position = snapPosition(worldPoint, 1.5f);
                if (Input.GetMouseButton(0)) {
                    if (validPlace("Plane")) {
                       wallCollection.Add(Instantiate(Wall, snapPosition(getWorldPoint(), 1.5f), rotationAngle));
                       //Debug.Log("Wall count: " + wallCollection.Count);
                    }
                }
            }

        }

        if (floorActive) {
            worldPoint = getWorldPoint();
            if (worldPoint != Vector3.zero) {
                FloorMouse.transform.position = snapPosition(getWorldPoint(), 0.5f);
                if (Input.GetMouseButton(0)) {
                    if (validPlace("Plane")) {
                        floorCollection.Add(Instantiate(Floor, snapPosition(getWorldPoint(), 0.5f), rotationAngle));
                        //Debug.Log("floor count: " + floorCollection.Count);
                    }
                }
            }
        }

        if (chestActive) {
            worldPoint = getWorldPoint();
            if (worldPoint != Vector3.zero) {
                ChestMouse.transform.position = snapPosition(getWorldPoint(), 1.5f);
                ChestMouse.transform.rotation = rotationAngle;
                if (Input.GetMouseButton(0)) {
                    if (validPlace("Floor")) {
                        chestCollection.Add(Instantiate(Chest, snapPosition(getWorldPoint(), 1.5f), rotationAngle));
                        //Debug.Log("Chest count: " + chestCollection.Count);
                    }
                }
            }

        }

        if (deleteActive) {
            getWorldPointDelete();
        }

    }

    void objectKeyboardListener() {
        if (Input.GetKeyUp(KeyCode.R)) {
            rotateObjects();
        }
    }

    public Vector3 getWorldPoint() {
        if (EventSystem.current.IsPointerOverGameObject())
            return Vector3.zero;

        Camera cam = GetComponent<Camera>();
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) {
            return hit.point;
        }
        return Vector3.zero;
    }

    public void getWorldPointDelete() {

        Camera cam = GetComponent<Camera>();
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetMouseButton(1)) {
            if (Physics.Raycast(ray, out hit)) {
                GameObject delObject = hit.collider.gameObject;
                if (delObject.name != "Plane") {
                    GameObject temp = delObject;
                    Destroy(delObject);
                    deleteObjectFromList(floorCollection, temp);

                }
            }
        }

    }

    bool validPlace(String validObject) {
        Camera cam = GetComponent<Camera>();
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) {
            String hitObject = hit.collider.gameObject.name.Replace("Prefab(Clone)", "");
            if (hitObject == validObject) {
                return true;
            }
        }

        return false;

    }

    public Vector3 snapPosition(Vector3 original,float yOffset) {
        Vector3 snapped;
        snapped.x = Mathf.Floor(original.x + 0.5f);
        snapped.y = yOffset;
        snapped.z = Mathf.Floor(original.z + 0.5f);
        return snapped;
    }

    void disableAll() {
        chestActive = false;
        wallActive = false;
        floorActive = false;
    }

    void resetObjectPositions() {
        WallMouse.transform.position = new Vector3(10, -10, 10);
        ChestMouse.transform.position = new Vector3(10, -10, 10);
        FloorMouse.transform.position = new Vector3(10, -10, 10);
    }



    void deleteObjectFromList(List<GameObject> list, GameObject delObj ) {
        int index = 0;
        bool isFound = false;
        while (index < list.Count || isFound) {
            if(list[index] == delObj) {
                isFound = true;
                list.RemoveAt(index);
            }
            index++;
        }
    }

    void appendObjectToFile(List<GameObject> list, String name) {

        foreach (GameObject currentObjct in list) {
            using (StreamWriter sw = File.AppendText(path)) {

                Vector3 pos = currentObjct.transform.position;
                Vector3 rot = currentObjct.transform.rotation.eulerAngles;
                sw.WriteLine(name + ", " + pos.x + ", " + pos.y + ", " + pos.z + ", " + rot.x + ", " + rot.y + ", " + rot.z);
            }
        }
    }

    void readFile() {

        eraseObjects(floorCollection);
        eraseObjects(wallCollection);
        eraseObjects(chestCollection);

        using (StreamReader sr = File.OpenText(path)) {
            while (!sr.EndOfStream) {
                String line = sr.ReadLine();
                String[] splitLine = line.Split(',');
                String name = splitLine[0];
                Vector3 position = new Vector3(float.Parse(splitLine[1]), float.Parse(splitLine[2]), float.Parse(splitLine[3]));
                Vector3 rotation = new Vector3(float.Parse(splitLine[4]), float.Parse(splitLine[5]), float.Parse(splitLine[6]));
                switch (name) {
                    case "Floor":
                        floorCollection.Add(Instantiate(Floor, position, Quaternion.Euler(rotation)));
                        break;
                    case "Wall":
                        wallCollection.Add(Instantiate(Wall, position, Quaternion.Euler(rotation)));
                        break;
                    case "Chest":
                        chestCollection.Add(Instantiate(Chest, position, Quaternion.Euler(rotation)));
                        break;
                    default:
                        Debug.Log("Tried to load " + name + "and was not found!");
                        break;
                    
                }
            }
            sr.Close();
        }
    }

    void eraseObjects(List<GameObject> list) {
        foreach (GameObject currentObject in list) {
            Destroy(currentObject);
        }
        list.Clear();
    }

    void createFile() {
        using (StreamWriter sw = File.CreateText(path)) {
            sw.Close();
        }
    }

    void loadAction() {
        readFile();
    }

    void wallAction() {
        resetObjectPositions();
        disableAll();
        wallActive = true;
    }

    void floorAction() {
        resetObjectPositions();
        disableAll();
        floorActive = true;
    }

    void chestAction() {
        resetObjectPositions();
        disableAll();
        chestActive = true;
    }

    void saveAction() {
        //creates blank file
        createFile();

        appendObjectToFile(floorCollection, "Floor");
        appendObjectToFile(wallCollection, "Wall");
        appendObjectToFile(chestCollection, "Chest");
    }

    // time-start-of-run || algorithm || floorplan unique ID || cleaning PCT || vaccum running time || minutes left

}