using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using TMPro;

public class ObjectPlacement : MonoBehaviour {

    public GameObject Chest, Wall, Floor, Rovac, Table2x2, Table2x4, Table2x6;
    public GameObject ChestMouse, WallMouse, FloorMouse, RovacMouse, Table2x2Mouse, Table2x4Mouse, Table2x6Mouse;
    public Material valid, notValid;

    bool chestActive = false;
    bool rovacActive = false;
    bool wallActive = false;
    bool floorActive = false;
    bool tablesActive = false;
    bool deleteActive = false;
    bool table2x2Active = true;
    bool table2x4Active = false;
    bool table2x6Active = false;

    int rotationDirection = 1;
    Quaternion rotationAngle = Quaternion.Euler(0.0f, 0.0f, 0.0f);

    List<GameObject> chestCollection = new List<GameObject>();
    List<GameObject> wallCollection = new List<GameObject>();
    List<GameObject> floorCollection = new List<GameObject>();
    List<GameObject> rovacCollection = new List<GameObject>();
    List<GameObject> table2x2Collection = new List<GameObject>();
    List<GameObject> table2x4Collection = new List<GameObject>();
    List<GameObject> table2x6Collection = new List<GameObject>();


    Cleaning cleaningobject;
    LineRenderer line;
    Color currentColor;

    Vector3 worldPoint = Vector3.zero;

    Vector3 startingPoint, endingPoint;

    public Button wallButton, floorButton, chestButton, rovacButton, saveButton, loadButton, deleteButton, tableButton;
    public TMP_Dropdown floorDropdown, tableDropdown;
    public TMP_Text floorCountText;

    string path = @"default.txt";
    //string path = @"test.txt";

    // Start is called before the first frame update
    void Start() {
        worldPoint = getWorldPoint();
        wallButton.GetComponent<Button>().onClick.AddListener(wallAction);
        floorButton.GetComponent<Button>().onClick.AddListener(floorAction);
        chestButton.GetComponent<Button>().onClick.AddListener(chestAction);
        deleteButton.GetComponent<Button>().onClick.AddListener(deleteAction);
        saveButton.GetComponent<Button>().onClick.AddListener(saveAction);
        loadButton.GetComponent<Button>().onClick.AddListener(loadAction);
        tableButton.GetComponent<Button>().onClick.AddListener(tableAction);
        rovacButton.GetComponent<Button>().onClick.AddListener(rovacAction);

        line = GameObject.Find("Line").GetComponent<LineRenderer>();

        floorDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate {
            FloorValueChanged(floorDropdown);
        });
        tableDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate {
            TableValueChanged(tableDropdown);
        });

        currentColor = new Color(0.36f, 0.25f, 0.2f);

        //line.widthCurve = 2;
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

        Ray ray = new Ray(startingPoint, endingPoint);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {

            // Debug.Log("Hit");
            //Debug.Log(hit.collider.gameObject.name);
        }
    }

    void placeObject() {
        if (wallActive) {
            worldPoint = getWorldPoint();
            if (worldPoint != Vector3.zero) {
                WallMouse.transform.position = snapPosition(worldPoint, 1.5f);
                if (Input.GetMouseButtonDown(0)) {
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
                if (Input.GetMouseButtonDown(0)) {
                    if (validPlace("Plane")) {
                        floorCollection.Add(Instantiate(Floor, snapPosition(getWorldPoint(), 0.5f), rotationAngle));
                        updateFloorCountText();
                    }

                }
            }

            if (Input.GetMouseButtonUp(0)) {
                changeColor();
            }

        }

        if (chestActive) {
            worldPoint = getWorldPoint();
            if (worldPoint != Vector3.zero) {
                ChestMouse.transform.position = snapPosition(getWorldPoint(), 1.5f);
                ChestMouse.transform.rotation = rotationAngle;
                if (Input.GetMouseButtonDown(0)) {
                    if (validPlace("Floor")) {
                        chestCollection.Add(Instantiate(Chest, snapPosition(getWorldPoint(), 1.5f), rotationAngle));
                        //Debug.Log("Chest count: " + chestCollection.Count);
                    }
                }
            }
        }

        if (rovacActive) {
            worldPoint = getWorldPoint();
            if (worldPoint != Vector3.zero) {
                RovacMouse.transform.position = snapPosition(getWorldPoint(), 1.1f);
                RovacMouse.transform.rotation = rotationAngle;
                if (Input.GetMouseButtonDown(0)) {
                    if (validPlace("Floor")) {
                        Rovac.transform.position = snapPosition(getWorldPoint(), 1.1f);
                    }
                }
            }
        }

        if (tablesActive) {
            worldPoint = getWorldPoint();
            if (worldPoint != Vector3.zero) {
                if (table2x2Active) {
                    temp(Table2x2Mouse, Table2x2, 1.45f, table2x2Collection);
                }
                if (table2x4Active) {
                    temp(Table2x4Mouse, Table2x4, 1.45f, table2x4Collection);
                }
                if (table2x6Active) {
                    temp(Table2x6Mouse, Table2x6, 1.45f, table2x6Collection);
                }
            }
        }

        if (deleteActive) {
            getWorldPointDelete();
        }
    }


    void temp(GameObject mouseObj, GameObject prefabObj, float yOffset, List<GameObject> collection) {
        mouseObj.transform.position = snapPosition(getWorldPoint(), yOffset);
        mouseObj.transform.rotation = rotationAngle;
        if (Input.GetMouseButtonDown(0)) {

            if (validPlace("Floor")) {
                collection.Add(Instantiate(prefabObj, snapPosition(getWorldPoint(), yOffset), rotationAngle));
            }
        }
    }

    void objectKeyboardListener() {
        if (Input.GetKeyUp(KeyCode.R)) {
            rotateObjects();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            startingPoint = snapPosition(getWorldPoint(), 0.5f);
            line.SetPosition(0, startingPoint);
            line.SetPosition(1, startingPoint);
        }

        if (Input.GetKeyDown(KeyCode.RightShift)) {
            endingPoint = snapPosition(getWorldPoint(), 0.5f);
            //Debug.DrawLine(startingPoint, endingPoint, Color.green, 20f, true);
            line.SetPosition(1, endingPoint);

            if ((int)startingPoint.x == (int)endingPoint.x || (int)startingPoint.z == (int)endingPoint.z) {
                line.material = valid;
            }
            else {
                line.material = notValid;
            }
        }

        if (Input.GetKeyDown(KeyCode.B)) {
            if (isStraightLine()) {

                if (isStraightXLine().valid) {
                    int lowest = Math.Min(isStraightXLine().startZ, isStraightXLine().endZ);
                    int highest = Math.Max(isStraightXLine().startZ, isStraightXLine().endZ);

                    for (int i = lowest; lowest <= highest; lowest++) {
                        floorCollection.Add(Instantiate(Floor, new Vector3(startingPoint.x, 0.5f, lowest), rotationAngle));
                    }
                    updateFloorCountText();
                }

                if (isStraightZLine().valid) {
                    int lowest = Math.Min(isStraightZLine().startX, isStraightZLine().endX);
                    int highest = Math.Max(isStraightZLine().startX, isStraightZLine().endX);

                    for (int i = lowest; lowest <= highest; lowest++) {
                        floorCollection.Add(Instantiate(Floor, new Vector3(lowest, 0.5f, startingPoint.z), rotationAngle));
                    }
                    updateFloorCountText();
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.B)) {
            changeColor();
        }


    }

    bool isStraightLine() {
        if ((int)startingPoint.x == (int)endingPoint.x || (int)startingPoint.z == (int)endingPoint.z) {
            return true;
        }
        else {
            return false;
        }
    }

    (bool valid, int startZ, int endZ) isStraightXLine() {
        if ((int)startingPoint.x == (int)endingPoint.x) {
            return (true, (int)startingPoint.z, (int)endingPoint.z);
        }
        else {
            return (false, (int)startingPoint.z, (int)endingPoint.z);
        }
    }

    (bool valid, int startX, int endX) isStraightZLine() {
        if ((int)startingPoint.z == (int)endingPoint.z) {
            return (true, (int)startingPoint.x, (int)endingPoint.x);
        }
        else {
            return (false, (int)startingPoint.x, (int)endingPoint.x);
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

        if (Input.GetMouseButtonDown(0)) {
            if (Physics.Raycast(ray, out hit)) {
                GameObject delObject = hit.collider.gameObject;
                String objType = removePrefabClone(delObject.name);
                Debug.Log(objType);
                if (objType != "Plane") {
                    switch (objType) {
                        case "Floor":
                            deleteObjectFromList(floorCollection, delObject);
                            updateFloorCountText();
                            break;
                        case "Chest":
                            deleteObjectFromList(chestCollection, delObject);
                            break;
                        case "Wall":
                            deleteObjectFromList(wallCollection, delObject);
                            break;
                        case "Table2x2":
                            Debug.Log("Here");
                            deleteObjectFromList(table2x2Collection, delObject);
                            break;
                        case "Table2x4":
                            deleteObjectFromList(table2x4Collection, delObject);
                            break;
                        case "Table2x6":
                            deleteObjectFromList(table2x6Collection, delObject);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

    }

    String removePrefabClone(String word) {
        return word.Replace("Prefab(Clone)", "");
    }

    bool validPlace(String validObject) {
        Camera cam = GetComponent<Camera>();
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) {
            String hitObject = removePrefabClone(hit.collider.gameObject.name);
            if (hitObject == validObject) {
                return true;
            }
        }

        return false;

    }

    public Vector3 snapPosition(Vector3 original, float yOffset) {
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
        tablesActive = false;
        rovacActive = false;
        deleteActive = false;
    }

    void resetObjectPositions() {
        WallMouse.transform.position = new Vector3(10, -10, 10);
        ChestMouse.transform.position = new Vector3(10, -10, 10);
        FloorMouse.transform.position = new Vector3(10, -10, 10);
        RovacMouse.transform.position = new Vector3(10, -10, 10);
        Table2x2Mouse.transform.position = new Vector3(10, -10, 10);
        Table2x4Mouse.transform.position = new Vector3(10, -10, 10);
        Table2x6Mouse.transform.position = new Vector3(10, -10, 10);
    }

    void deleteObjectFromList(List<GameObject> list, GameObject delObj) {
        for (int index = 0; index < list.Count; index++) {
            if (list[index].GetInstanceID() == delObj.GetInstanceID()) {
                list.Remove(delObj);
                Destroy(delObj);
            }
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
        updateFloorCountText();
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

    void updateFloorCountText() {
        if (floorCollection.Count >= 200 && floorCollection.Count <= 8000) {
            floorCountText.color = Color.green;
        }
        else {
            floorCountText.color = Color.red;
        }
        floorCountText.text = $"Square Feet: {floorCollection.Count}";
    }

    void FloorValueChanged(TMP_Dropdown change) {
        switchFloorSettings(change.value);
    }

    void TableValueChanged(TMP_Dropdown change) {
        switchTableSettings(change.value);
    }

    void switchFloorSettings(int choice) {
        switch (choice) {
            case 0:
                currentColor = new Color(0.36f, 0.25f, 0.2f);
                changeFloorMouseColor(FloorMouse, currentColor);
                break;
            case 1:
                currentColor = new Color(0f, 0.39f, 0f);
                changeFloorMouseColor(FloorMouse, currentColor);
                break;
            case 2:
                currentColor = new Color(1f, 0.4f, 0f);
                changeFloorMouseColor(FloorMouse, currentColor);
                break;
            case 3:
                currentColor = new Color(1f, 0f, 0f);
                changeFloorMouseColor(FloorMouse, currentColor);
                break;
            default:
                break;
        }

    }

    void resetTableActive() {
        table2x2Active = false;
        table2x4Active = false;
        table2x6Active = false;
    }

    void switchTableSettings(int choice) {
        switch (choice) {
            case 0:
                resetObjectPositions();
                resetTableActive();
                table2x2Active = true;
                break;
            case 1:
                resetObjectPositions();
                resetTableActive();
                table2x4Active = true;
                break;
            case 2:
                resetObjectPositions();
                resetTableActive();
                table2x6Active = true;
                break;
            default:
                break;
        }

    }

    void changeFloorMouseColor(GameObject g, Color c) {
        g.GetComponent<Renderer>().material.color = c;
    }

    void changeColor() {
        int numberOfFloors = floorCollection.Count;
        for (int i = numberOfFloors - 1; i > 0; i--) {
            if (floorCollection[i].GetComponent<Renderer>().material.color != currentColor) {
                floorCollection[i].GetComponent<Renderer>().material.color = currentColor;
            }
            else {
                break;
            }
        }
    }

    void tableAction() {
        resetObjectPositions();
        disableAll();
        tablesActive = true;
    }

    void rovacAction() {
        resetObjectPositions();
        disableAll();
        rovacActive = true;
    }

    void deleteAction() {
        resetObjectPositions();
        disableAll();
        deleteActive = true;
    }


    // time-start-of-run || algorithm || floorplan unique ID || cleaning PCT || vaccum running time || minutes left

}