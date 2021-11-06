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
    int rotationDirection = 1;
    Quaternion rotationAngle = Quaternion.Euler(0.0f, 0.0f, 0.0f);

    Stack chestCollection = new Stack();
    Stack wallCollection = new Stack();
    Stack floorCollection = new Stack();

    Vector3 worldPoint = Vector3.zero;

    public Button wallButton, floorButton, chestButton, saveButton, loadButton;

    string path = @"default.txt";

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
        Debug.Log(rotationDirection);
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
                WallMouse.transform.position = wallSnapPosition(worldPoint);
                if (Input.GetMouseButtonDown(0)) {
                    wallCollection.Push(Instantiate(Wall, wallSnapPosition(getWorldPoint()), rotationAngle));
                }
            }

        }

        if (floorActive) {
            worldPoint = getWorldPoint();
            if (worldPoint != Vector3.zero) {
                FloorMouse.transform.position = floorsnapPosition(getWorldPoint());
                if (Input.GetMouseButtonDown(0)) {
                    floorCollection.Push(Instantiate(Floor, floorsnapPosition(getWorldPoint()), rotationAngle));
                }
            }
        }

        if (chestActive) {
            worldPoint = getWorldPoint();
            if (worldPoint != Vector3.zero) {
                ChestMouse.transform.position = chestSnapPosition(getWorldPoint());
                ChestMouse.transform.rotation = rotationAngle;
                if (Input.GetMouseButtonDown(0)) {
                    chestCollection.Push(Instantiate(Chest, chestSnapPosition(getWorldPoint()), rotationAngle));
                }
            }

        }

    }

    void objectKeyboardListener() {
        if (Input.GetKeyUp(KeyCode.W)) {
            resetObjectPositions();
            disableAll();
            wallActive = true;
        }
        if (Input.GetKeyUp(KeyCode.C)) {
            resetObjectPositions();
            disableAll();
            chestActive = true;
        }
        if (Input.GetKeyUp(KeyCode.F)) {
            resetObjectPositions();
            disableAll();
            floorActive = true;
        }
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
            //Debug.Log(hit.collider.name);
            //if (hit.collider.name == "Plane") {//
              return hit.point;
            //}
            //else {
             //   return Vector3.zero;
           //}
        }
        return Vector3.zero;
    }

    public Vector3 floorsnapPosition(Vector3 original) {
        Vector3 snapped;
        snapped.x = Mathf.Floor(original.x + 0.5f);
        snapped.y = 0.5f;
        snapped.z = Mathf.Floor(original.z + 0.5f);
        return snapped;
    }

    public Vector3 wallSnapPosition(Vector3 original) {
        Vector3 snapped;
        snapped.x = Mathf.Floor(original.x + 0.5f);
        snapped.y = 1.5f;
        snapped.z = Mathf.Floor(original.z + 0.5f);
        return snapped;
    }

    public Vector3 chestSnapPosition(Vector3 original) {
        Vector3 snapped;
        snapped.x = Mathf.Floor(original.x + 0.5f);
        snapped.y = 1.5f;
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
        ChestMouse.transform.position = new Vector3(0, -10, 0);
        FloorMouse.transform.position = new Vector3(5, -10, 5);
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

        /* 
        foreach (GameObject floor in floorCollection) {
            Vector3 fObjPos = floor.transform.position;
            Vector3 fObjRot = floor.transform.rotation.eulerAngles;
            Debug.Log(fObjPos.x.ToString() + " " + fObjPos.y.ToString() + " " + fObjPos.z.ToString() + "   |   " + fObjRot.x.ToString() + " / " + fObjRot.y.ToString() + " / " + fObjRot.z.ToString());
        }

        foreach (GameObject chest in chestCollection) {
            Vector3 fObjPos = chest.transform.position;
            Vector3 fObjRot = chest.transform.rotation.eulerAngles;
        }
        */
    }

    void appendObjectToFile(Stack stack, String name) {
        // StreamWriter sw;
        // File file;

        foreach (GameObject currentObjct in stack) {
            using (StreamWriter sw = File.AppendText(path)) {

                Vector3 pos = currentObjct.transform.position;
                Vector3 rot = currentObjct.transform.rotation.eulerAngles;
                sw.WriteLine(name + ", " + pos.x + ", "  + pos.y + ", " + pos.z + ", " +  rot.x + ", " + rot.y + ", " + rot.z);
                //sw.Close();
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
                Debug.Log(line);
                String[] splitLine = line.Split(',');
                Vector3 position = new Vector3(float.Parse(splitLine[1]), float.Parse(splitLine[2]), float.Parse(splitLine[3]));
                Vector3 rotation = new Vector3(float.Parse(splitLine[4]), float.Parse(splitLine[5]), float.Parse(splitLine[6]));
                if (splitLine[0] == "Floor") {
                    floorCollection.Push(Instantiate(Floor, position, Quaternion.Euler(rotation)));
                }
                if (splitLine[0] == "Wall") {
                    wallCollection.Push(Instantiate(Wall, position, Quaternion.Euler(rotation)));
                }
                if (splitLine[0] == "Chest") {
                    chestCollection.Push(Instantiate(Chest, position, Quaternion.Euler(rotation)));
                }
            }
            sr.Close();
        }
    }

    void eraseObjects(Stack stack) {
        foreach(GameObject currentObject in stack) {
            Destroy(currentObject);
        }
        stack.Clear();
    }

    void createFile() {
        using (StreamWriter sw = File.CreateText(path)) {
            sw.Close();
        }
    }

    void loadAction() {
        readFile();
    }

    void resetStack(Stack s) {
        s.Clear();
    }

    class SimData {

        public Boolean fileExist = false;
        public string globalPath = null;

        /******************************************
         * 
         * Function tests to see if a file exists at the path inserted through the parameter (path)
         * 
        ******************************************/
        public void clearOldFile() {
            fileExist = false;
        }

        public void setGlobalPath(string path) {
            globalPath = path;
        }

        public string createCSVDataLine(string objectname, float posX, float posY, float posZ, float rotX, float rotY, float rotZ) {
            string dataLine = objectname + ", " + posX + ", " + posY + ", " + posZ + ", " + rotX + ", " + rotY + ", " + rotZ;
            return dataLine;

        }

        public void writeCSV(string dataLine) {
            if (!fileExist && globalPath != null) {
                using (StreamWriter sw = File.CreateText(globalPath)) {
                    sw.WriteLine(dataLine);
                    sw.Close();
                }
                fileExist = true;
            }
            else if (fileExist && globalPath != null) {
                using (StreamWriter sw = File.AppendText(globalPath)) {
                    sw.WriteLine(dataLine);
                    sw.Close();
                }

            }
            else {
            }
        }

        public List<RoomFormData> readCSV() {
            List<RoomFormData> retList = new List<RoomFormData>();
            using (StreamReader sr = File.OpenText(globalPath)) {
                while (!sr.EndOfStream) {
                    String line = sr.ReadLine();
                    String[] splitLine = line.Split(',');
                    RoomFormData dataObj = new RoomFormData(splitLine[0], float.Parse(splitLine[1]), float.Parse(splitLine[2]), float.Parse(splitLine[3]), float.Parse(splitLine[4]), float.Parse(splitLine[5]), float.Parse(splitLine[6]));
                    retList.Add(dataObj);
                }
                sr.Close();
            }
            return retList;
        }


    }
    class RoomFormData {
        public string ObjectName { get; set; }
        public float posX { get; set; }
        public float posY { get; set; }
        public float posZ { get; set; }
        public float rotX { get; set; }
        public float rotY { get; set; }
        public float rotZ { get; set; }

        public RoomFormData(string name, float xPos, float yPos, float zPos, float xRot, float yRot, float zRot) {
            this.ObjectName = name;
            this.posX = xPos;
            this.posY = yPos;
            this.posZ = zPos;
            this.rotX = xRot;
            this.rotY = yRot;
            this.rotZ = zRot;
        }

    }

}