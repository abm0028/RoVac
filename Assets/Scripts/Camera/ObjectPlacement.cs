using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using TMPro;

public class ObjectPlacement : MonoBehaviour {

    private Rect windowRect = new Rect((Screen.width - 200) / 2, (Screen.height - 300) / 2, 200, 75);
    // creates mouse and actual objects
    public GameObject Chest, Wall, Floor, Rovac, Table2x2, Table2x4, Table2x6, Chair2x2, Chair2x4;
    public GameObject ChestMouse, WallMouse, FloorMouse, RovacMouse, Table2x2Mouse, Table2x4Mouse, Table2x6Mouse, Chair2x2Mouse, Chair2x4Mouse;
    // material used for the line in bulk mode
    // valid is green
    // invalid is red
    public Material valid, notValid;

    // y offset for the objects when they are placed
    float wallYOffset = 1.5f;
    float tablesYOffset = 1.45f;
    float floorYOffset = 0.5f;
    float chairYOffset = 2.25f;
    float chestYOffset = 1.5f;

    // bools to see which object is active to be placed
    bool chestActive = false;
    bool rovacActive = false;
    bool wallActive = false;
    bool floorActive = false;
    bool tablesActive = false;
    bool chairsActive = false;
    bool deleteActive = false;
    bool bulkActive = false;
    bool table2x2Active = true;
    bool table2x4Active = false;
    bool table2x6Active = false;
    bool chair2x2Active = true;
    bool chair2x4Active = false;

    // counts how many bulk clicks have been made
    int bulkCicks = 0;
    // default linegiht for the line in bulk mode
    float bulkRaycastLineHeight = 4f;

    int rotationDirection = 1;
    Quaternion rotationAngle = Quaternion.Euler(0.0f, 0.0f, 0.0f);

    // creates the lists that hold objects
    // floor is public because it needs to be accesed in other classes
    public List<GameObject> floorCollection = new List<GameObject>();
    List<GameObject> chestCollection = new List<GameObject>();
    List<GameObject> wallCollection = new List<GameObject>();
    List<GameObject> table2x2Collection = new List<GameObject>();
    List<GameObject> table2x4Collection = new List<GameObject>();
    List<GameObject> table2x6Collection = new List<GameObject>();
    List<GameObject> chair2x2Collection = new List<GameObject>();
    List<GameObject> chair2x4Collection = new List<GameObject>();

    // create line for bulk mode
    LineRenderer line;
    // Vectors for the starting and ending points of the line for bulk mode
    Vector3 startingPoint, endingPoint;

    // defaults worldpoint to zero
    Vector3 worldPoint = Vector3.zero;
    Color floorCurrentColor;

    private bool show = false;
    public GUIStyle primaryButtonSkin;
    public GUIStyle secondaryButtonSkin;


    // objexcts for the UI elements
    public Button wallButton, floorButton, chestButton, rovacButton, saveButton, loadButton, deleteButton, tableButton, bulkButton, chairButton, startButton;
    public TMP_Dropdown floorDropdown, tableDropdown, chairDropdown, loadDropdown;
    public TMP_Text floorCountText;
    string loadPath, savePath;

    string setPath(string path) {
        if (Application.isEditor) {
            return $@"Assets/Resources/{path}";
        } else {
            return $"{Application.dataPath}/StreamingAssets/{path}";
        }
    }


    // Start is called before the first frame update
    void Start() {
        loadPath = setPath("userhouse.txt");
        savePath = setPath("userhouse.txt");
        worldPoint = getWorldPoint();

        // adds the listeneres to the UI elements
        wallButton.GetComponent<Button>().onClick.AddListener(wallAction);
        floorButton.GetComponent<Button>().onClick.AddListener(floorAction);
        chestButton.GetComponent<Button>().onClick.AddListener(chestAction);
        deleteButton.GetComponent<Button>().onClick.AddListener(deleteAction);
        saveButton.GetComponent<Button>().onClick.AddListener(saveAction);
        loadButton.GetComponent<Button>().onClick.AddListener(loadAction);
        tableButton.GetComponent<Button>().onClick.AddListener(tableAction);
        chairButton.GetComponent<Button>().onClick.AddListener(chairAction);
        rovacButton.GetComponent<Button>().onClick.AddListener(rovacAction);
        bulkButton.GetComponent<Button>().onClick.AddListener(bulkAction);
        startButton.GetComponent<Button>().onClick.AddListener(startAction);
        floorDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate {
            FloorValueChanged(floorDropdown);
        });
        tableDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate {
            TableValueChanged(tableDropdown);
        });
        chairDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate {
            ChairValueChanged(chairDropdown);
        });
        loadDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate {
            LoadValueChanged(loadDropdown);
        });

        // init line renderer for bulk mode
        line = GameObject.Find("Line").GetComponent<LineRenderer>();
        floorCurrentColor = new Color(0.36f, 0.25f, 0.2f);
    }


    // rotates objects
    void rotateObjects() {
        if (rotationDirection == 4) {
            rotationDirection = 1;
        } else {
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

    // returns the floor count of the floorcollection
    public int getFloorCount() {
        return floorCollection.Count;
    }

    // gets the average dirtiness of all the floor tiles in the simulation
    public float getAverages() {
        int count = floorCollection.Count;
        float sum = 0.0f;

        foreach (GameObject Floor in floorCollection) {
            sum += Floor.GetComponent<Cleaning>().getPercentage();
        }

        float average = 1 - (sum / (float)count);
        return average;
    }

    // places objects
    void placeObject() {

        // if wall is selected
        if (wallActive) {
            worldPoint = getWorldPoint();
            if (worldPoint != Vector3.zero) {
                // if is in bulk mode do not render wall on mouse position
                if (!bulkActive) {
                    // snaps position to the grid
                    WallMouse.transform.position = snapPosition(worldPoint, wallYOffset);
                }
                // if mouse is on plane then it it will addd the wall to the list and add to world
                if (Input.GetMouseButtonDown(0)) {
                    if (validPlace("Plane")) {
                        wallCollection.Add(Instantiate(Wall, snapPosition(getWorldPoint(), wallYOffset), rotationAngle));
                    }
                }
            }
        }

        // if floor is selected
        if (floorActive) {
            worldPoint = getWorldPoint();
            if (worldPoint != Vector3.zero) {
                // if is in bulk mode do not render floor on mouse position
                if (!bulkActive) {
                    // snaps position to the grid
                    FloorMouse.transform.position = snapPosition(getWorldPoint(), floorYOffset);
                }
                // if mouse is on plane then it it will addd the floor to the list and add to world
                if (Input.GetMouseButtonDown(0)) {
                    if (validPlace("Plane")) {
                        floorCollection.Add(Instantiate(Floor, snapPosition(getWorldPoint(), floorYOffset), rotationAngle));
                        // updates the text of the 
                        updateFloorCountText();
                    }

                }
            }
            // when left mouse goes up change the color of the floor objects that are still the original color
            if (Input.GetMouseButtonUp(0)) {
                changeColor();
            }

        }

        // if chest is selected
        if (chestActive) {
            worldPoint = getWorldPoint();
            if (worldPoint != Vector3.zero) {
                // snaps position to the grid
                ChestMouse.transform.position = snapPosition(getWorldPoint(), chestYOffset);
                ChestMouse.transform.rotation = rotationAngle;
                if (Input.GetMouseButtonDown(0)) {
                    if (validPlace("Floor")) {
                        // places chest on the world and in the chest collection list
                        chestCollection.Add(Instantiate(Chest, snapPosition(getWorldPoint(), chestYOffset), rotationAngle));
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

        // if table is selected
        if (tablesActive) {
            worldPoint = getWorldPoint();
            if (worldPoint != Vector3.zero) {
                if (table2x2Active) {
                    placeObjectVariant(Table2x2Mouse, Table2x2, tablesYOffset, table2x2Collection);
                }
                if (table2x4Active) {
                    placeObjectVariant(Table2x4Mouse, Table2x4, tablesYOffset, table2x4Collection);
                }
                if (table2x6Active) {
                    placeObjectVariant(Table2x6Mouse, Table2x6, tablesYOffset, table2x6Collection);
                }
            }
        }

        // if chairs is selected
        if (chairsActive) {
            worldPoint = getWorldPoint();
            if (worldPoint != Vector3.zero) {
                if (chair2x2Active) {
                    placeObjectVariant(Chair2x2Mouse, Chair2x2, chairYOffset, chair2x2Collection);
                }
                if (chair2x4Active) {
                    placeObjectVariant(Chair2x4Mouse, Chair2x4, chairYOffset, chair2x4Collection);
                }
            }
        }


        // if delete mode is active
        if (deleteActive) {
            getWorldPointDelete();
        }
    }

    // places tables and chairs variants
    void placeObjectVariant(GameObject mouseObj, GameObject prefabObj, float yOffset, List<GameObject> collection) {
        mouseObj.transform.position = snapPosition(getWorldPoint(), yOffset);
        mouseObj.transform.rotation = rotationAngle;
        if (Input.GetMouseButtonDown(0)) {
            if (validPlace("Floor")) {
                collection.Add(Instantiate(prefabObj, snapPosition(getWorldPoint(), yOffset), rotationAngle));
            }
        }
    }

    void objectKeyboardListener() {
        // if user presses r key then it will rotate the objects
        if (Input.GetKeyUp(KeyCode.R)) {
            rotateObjects();
        }

        // if bulk active is on
        if (bulkActive) {

            // first right click in bulk mode
            // sets the start position of the bulk mode
            if (Input.GetMouseButtonDown(1) && bulkCicks == 0) {
                startingPoint = snapPosition(getWorldPoint(), bulkRaycastLineHeight);
                line.SetPosition(0, startingPoint);
                line.SetPosition(1, startingPoint);
                bulkCicks = 1;
            }

            // after first line point has been placed
            if (bulkCicks == 1) {

                endingPoint = snapPosition(getWorldPoint(), bulkRaycastLineHeight);
                line.SetPosition(1, endingPoint);

                // checls to see if two points are on same x or z axis and changtes color to green for valid and red for invalid
                if ((int)startingPoint.x == (int)endingPoint.x || (int)startingPoint.z == (int)endingPoint.z) {
                    line.material = valid;
                } else {
                    line.material = notValid;
                }

                // when pressed
                if (Input.GetMouseButtonDown(1)) {

                    // if it is a straight line then it will place the objects
                    if (isStraightLine()) {

                        // if is on the x axis it will place objects on the x axis
                        if (isStraightXLine().valid) {
                            int lowest = Math.Min(isStraightXLine().startZ, isStraightXLine().endZ);
                            int highest = Math.Max(isStraightXLine().startZ, isStraightXLine().endZ);

                            if (lowest != highest) {
                                if (floorActive) {
                                    for (int i = lowest; lowest <= highest; lowest++) {
                                        floorCollection.Add(Instantiate(Floor, new Vector3(startingPoint.x, floorYOffset, lowest), rotationAngle));
                                    }
                                    //resets the bulk line after objects placed
                                    resetBulkLine();
                                    updateFloorCountText();
                                }

                                if (wallActive) {
                                    for (int i = lowest; lowest <= highest; lowest++) {
                                        wallCollection.Add(Instantiate(Wall, new Vector3(startingPoint.x, wallYOffset, lowest), rotationAngle));
                                    }
                                    resetBulkLine();
                                }
                            }
                        }

                        if (isStraightZLine().valid) {
                            int lowest = Math.Min(isStraightZLine().startX, isStraightZLine().endX);
                            int highest = Math.Max(isStraightZLine().startX, isStraightZLine().endX);

                            if (lowest != highest) {
                                if (floorActive) {

                                    for (int i = lowest; lowest <= highest; lowest++) {
                                        floorCollection.Add(Instantiate(Floor, new Vector3(lowest, floorYOffset, startingPoint.z), rotationAngle));
                                    }
                                    updateFloorCountText();
                                    resetBulkLine();
                                }

                                if (wallActive) {
                                    for (int i = lowest; lowest <= highest; lowest++) {
                                        wallCollection.Add(Instantiate(Wall, new Vector3(lowest, wallYOffset, startingPoint.z), rotationAngle));
                                    }
                                    resetBulkLine();
                                }
                            }
                        }

                        if (floorActive == false && wallActive == false) {
                            resetBulkLine();
                        }
                    }
                }
            }
            // chnages color of the floors that have not been changed to current floor color on letting go of right click
            if (Input.GetMouseButtonUp(1)) {
                changeColor();
            }
        }
    }

    // resets bulk lines
    void resetBulkLine() {
        bulkCicks = 0;
        startingPoint = Vector3.zero;
        endingPoint = Vector3.zero;
        line.SetPosition(0, startingPoint);
        line.SetPosition(1, startingPoint);
    }

    bool isStraightLine() {
        if ((int)startingPoint.x == (int)endingPoint.x || (int)startingPoint.z == (int)endingPoint.z) {
            return true;
        } else {
            return false;
        }
    }

    // checks if the line is straight on the x axis
    (bool valid, int startZ, int endZ) isStraightXLine() {
        if ((int)startingPoint.x == (int)endingPoint.x) {
            return (true, (int)startingPoint.z, (int)endingPoint.z);
        } else {
            return (false, (int)startingPoint.z, (int)endingPoint.z);
        }
    }

    // checks if the line is straight on the z axis
    (bool valid, int startX, int endX) isStraightZLine() {
        if ((int)startingPoint.z == (int)endingPoint.z) {
            return (true, (int)startingPoint.x, (int)endingPoint.x);
        } else {
            return (false, (int)startingPoint.x, (int)endingPoint.x);
        }
    }

    // gets the world point of the mousse
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

    // gets world point of the mouse and delete object when left clicks
    public void getWorldPointDelete() {

        Camera cam = GetComponent<Camera>();
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0)) {
            if (Physics.Raycast(ray, out hit)) {
                GameObject delObject = hit.collider.gameObject;
                String objType = removePrefabClone(delObject.name);

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
                            deleteObjectFromList(table2x2Collection, delObject);
                            break;
                        case "Table2x4":
                            deleteObjectFromList(table2x4Collection, delObject);
                            break;
                        case "Table2x6":
                            deleteObjectFromList(table2x6Collection, delObject);
                            break;
                        case "Chair2x2":
                            deleteObjectFromList(chair2x2Collection, delObject);
                            break;
                        case "Chair2x4":
                            deleteObjectFromList(chair2x4Collection, delObject);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

    }

    //remvoe "prefab(clone)" from the gameobject name
    String removePrefabClone(String word) {
        return word.Replace("Prefab(Clone)", "");
    }

    // checks to see if valid place to place objext
    // example validPlace(floor) checks to see if pointing at floor
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

    // snaps mouse posistion on a grid 
    public Vector3 snapPosition(Vector3 original, float yOffset) {
        Vector3 snapped;
        snapped.x = Mathf.Floor(original.x + 0.5f);
        snapped.y = yOffset;
        snapped.z = Mathf.Floor(original.z + 0.5f);
        return snapped;
    }

    //disables all objects to make sure only one is active at a time
    void disableAll() {
        chestActive = false;
        wallActive = false;
        floorActive = false;
        tablesActive = false;
        rovacActive = false;
        deleteActive = false;
        chairsActive = false;
    }

    // resets the mouse objext so they do not stay on the mouse after another has been activated
    void resetObjectPositions() {
        WallMouse.transform.position = new Vector3(10, -10, 10);
        ChestMouse.transform.position = new Vector3(10, -10, 10);
        FloorMouse.transform.position = new Vector3(10, -10, 10);
        RovacMouse.transform.position = new Vector3(10, -10, 10);
        Table2x2Mouse.transform.position = new Vector3(10, -10, 10);
        Table2x4Mouse.transform.position = new Vector3(10, -10, 10);
        Table2x6Mouse.transform.position = new Vector3(10, -10, 10);
        Chair2x2Mouse.transform.position = new Vector3(10, -10, 10);
        Chair2x4Mouse.transform.position = new Vector3(10, -10, 10);
    }

    // deletes object from list and destory it from the simulation and memory
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
            using (StreamWriter sw = File.AppendText(savePath)) {
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
        eraseObjects(table2x2Collection);
        eraseObjects(table2x2Collection);
        eraseObjects(table2x6Collection);
        eraseObjects(chair2x2Collection);
        eraseObjects(chair2x4Collection);

        Debug.Log(loadPath);

        using (StreamReader sr = File.OpenText(loadPath)) {
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
                    case "Table2x2":
                        table2x2Collection.Add(Instantiate(Table2x2, position, Quaternion.Euler(rotation)));
                        break;
                    case "Table2x4":
                        table2x4Collection.Add(Instantiate(Table2x4, position, Quaternion.Euler(rotation)));
                        break;
                    case "Table2x6":
                        table2x6Collection.Add(Instantiate(Table2x6, position, Quaternion.Euler(rotation)));
                        break;
                    case "Chair2x2":
                        chair2x2Collection.Add(Instantiate(Chair2x2, position, Quaternion.Euler(rotation)));
                        break;
                    case "Chair2x4":
                        chair2x4Collection.Add(Instantiate(Chair2x4, position, Quaternion.Euler(rotation)));
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
        using (StreamWriter sw = File.CreateText(loadPath)) {
            sw.Close();
        }
    }

    void loadAction() {
        readFile();
    }

    void wallAction() {
        resetObjectPositions();
        disableAll();
        bulkRaycastLineHeight = wallYOffset;
        wallActive = true;
    }

    void floorAction() {
        resetObjectPositions();
        disableAll();
        bulkRaycastLineHeight = floorYOffset;
        floorActive = true;
    }

    void chestAction() {
        resetObjectPositions();
        disableAll();
        chestActive = true;
    }

    void saveAction() {
        //creates blank file
        show = true;
    }


    void DialogWindow(int windowID) {
        float y = 20;

        if (GUI.Button(new Rect(5, y, windowRect.width - 10, 20), "No", secondaryButtonSkin)) {
            show = false;
        }

        if (GUI.Button(new Rect(5, y + 30, windowRect.width - 10, 20), "Yes", primaryButtonSkin)) {
            createFile();
            appendObjectToFile(floorCollection, "Floor");
            appendObjectToFile(wallCollection, "Wall");
            appendObjectToFile(chestCollection, "Chest");
            appendObjectToFile(table2x2Collection, "Table2x2");
            appendObjectToFile(table2x4Collection, "Table2x4");
            appendObjectToFile(table2x6Collection, "Table2x6");
            appendObjectToFile(chair2x2Collection, "Chair2x2");
            appendObjectToFile(chair2x4Collection, "Chair2x4");
            show = false;
        }
    }

    void OnGUI() {
        if (show)
            windowRect = GUI.Window(0, windowRect, DialogWindow, "You sure you want to save?");
    }

    void updateFloorCountText() {
        if (floorCollection.Count >= 200 && floorCollection.Count <= 8000) {
            floorCountText.color = Color.green;
        } else {
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

    void ChairValueChanged(TMP_Dropdown change) {
        switchChairSettings(change.value);
    }

    void LoadValueChanged(TMP_Dropdown change) {
        switchLoadSettings(change.value);
    }

    void switchLoadSettings(int choice) {
        switch (choice) {
            case 0:
                loadPath = setPath("userhouse.txt");
                break;
            case 1:
                loadPath = setPath("housepreset1.txt");
                break;
            case 2:
                loadPath = setPath("housepreset2.txt");
                break;
            case 3:
                loadPath = setPath("housepreset3.txt");
                break;
            default:
                break;
        }
    }

    void switchChairSettings(int choice) {
        switch (choice) {
            case 0:
                resetObjectPositions();
                resetChairActive();
                chair2x2Active = true;
                break;
            case 1:
                resetObjectPositions();
                resetChairActive();
                chair2x4Active = true;
                break;
            default:
                break;
        }

    }

    void switchFloorSettings(int choice) {
        switch (choice) {
            case 0:
                floorCurrentColor = new Color(0.36f, 0.25f, 0.2f);
                changeFloorMouseColor(FloorMouse, floorCurrentColor);
                break;
            case 1:
                floorCurrentColor = new Color(0f, 0.39f, 0f);
                changeFloorMouseColor(FloorMouse, floorCurrentColor);
                break;
            case 2:
                floorCurrentColor = new Color(1f, 0.4f, 0f);
                changeFloorMouseColor(FloorMouse, floorCurrentColor);
                break;
            case 3:
                floorCurrentColor = new Color(1f, 0f, 0f);
                changeFloorMouseColor(FloorMouse, floorCurrentColor);
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

    void resetChairActive() {
        chair2x2Active = false;
        chair2x4Active = false;
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
            if (floorCollection[i].GetComponent<Renderer>().material.color != floorCurrentColor) {
                floorCollection[i].GetComponent<Renderer>().material.color = floorCurrentColor;
            } else {
                break;
            }
        }
    }

    void tableAction() {
        resetObjectPositions();
        disableAll();
        tablesActive = true;
    }

    void chairAction() {
        resetObjectPositions();
        disableAll();
        chairsActive = true;
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

    void startAction() {
        resetObjectPositions();
        disableAll();
    }

    void bulkAction() {
        resetObjectPositions();
        deleteActive = false;
        if (!bulkActive) {
            bulkActive = true;
        } else {
            bulkActive = false;
            resetBulkLine();
        }

    }


    // time-start-of-run || algorithm || floorplan unique ID || cleaning PCT || vaccum running time || minutes left

}