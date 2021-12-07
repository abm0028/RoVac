using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {
    public Button chairButton, tableButton, chestButton, floorButton, wallButton, rovacButton, saveButton, loadButton, deleteButton, bulkButton, exitButton, stopButton, startButton, menuButton;
    public TMP_Dropdown chairDropdown, tableDropdown, speedDropdown, floorDropdown, algorithmDropdown, loadDropdown;
    public TMP_InputField IDField;
    bool bulkActive = false;
    bool hasStarted = false;
    ObjectPlacement objectscript;
    public Camera cameraobj;
    int simSpeed = 1;

    private Rect windowRect = new Rect((Screen.width - 200) / 2, (Screen.height - 300) / 2, 200, 75);

    // Only show it if needed.
    private bool show = false;

    public GUIStyle primaryButtonSkin;
    public GUIStyle secondaryButtonSkin;

    // handles are you sure you want to exit dialog box
    void DialogWindow(int windowID) {
        float y = 20;

        if (GUI.Button(new Rect(5, y, windowRect.width - 10, 20), "No", secondaryButtonSkin)) {
            show = false;
        }

        if (GUI.Button(new Rect(5, y + 30, windowRect.width - 10, 20), "Yes", primaryButtonSkin)) {
            Application.Quit();
            show = false;
        }
    }

    void OnGUI() {
        if (show)
            windowRect = GUI.Window(0, windowRect, DialogWindow, "You sure you want to exit?");
    }


    // Start is called before the first frame update
    void Start() {

        chairButton.GetComponent<Button>().onClick.AddListener(chairAction);
        tableButton.GetComponent<Button>().onClick.AddListener(tableAction);
        chestButton.GetComponent<Button>().onClick.AddListener(chestAction);
        floorButton.GetComponent<Button>().onClick.AddListener(floorAction);
        wallButton.GetComponent<Button>().onClick.AddListener(wallAction);
        rovacButton.GetComponent<Button>().onClick.AddListener(rovacAction);
        deleteButton.GetComponent<Button>().onClick.AddListener(deleteAction);
        bulkButton.GetComponent<Button>().onClick.AddListener(bulkAction);
        exitButton.GetComponent<Button>().onClick.AddListener(exitAction);
        menuButton.GetComponent<Button>().onClick.AddListener(menuAction);
        stopButton.GetComponent<Button>().onClick.AddListener(stopAction);
        startButton.GetComponent<Button>().onClick.AddListener(startAction);

        speedDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate {
            SpeedValueChanged(speedDropdown);
        });

        stopButton.gameObject.SetActive(false);

    }

    public void hideUI() {
        chairButton.gameObject.SetActive(false);
        tableButton.gameObject.SetActive(false);
        chestButton.gameObject.SetActive(false);
        floorButton.gameObject.SetActive(false);
        wallButton.gameObject.SetActive(false);
        rovacButton.gameObject.SetActive(false);
        saveButton.gameObject.SetActive(false);
        loadButton.gameObject.SetActive(false);
        deleteButton.gameObject.SetActive(false);
        bulkButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
        chairDropdown.gameObject.SetActive(false);
        tableDropdown.gameObject.SetActive(false);
        speedDropdown.gameObject.SetActive(false);
        floorDropdown.gameObject.SetActive(false);
        loadDropdown.gameObject.SetActive(false);
        algorithmDropdown.gameObject.SetActive(false);
        tableDropdown.gameObject.SetActive(false);
        chairDropdown.gameObject.SetActive(false);
        IDField.gameObject.SetActive(false);
        menuButton.gameObject.SetActive(false);
        stopButton.gameObject.SetActive(true);

        this.gameObject.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    }

    public void showUI() {
        chairButton.gameObject.SetActive(true);
        tableButton.gameObject.SetActive(true);
        chestButton.gameObject.SetActive(true);
        floorButton.gameObject.SetActive(true);
        wallButton.gameObject.SetActive(true);
        rovacButton.gameObject.SetActive(true);
        saveButton.gameObject.SetActive(true);
        loadButton.gameObject.SetActive(true);
        deleteButton.gameObject.SetActive(true);
        bulkButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);
        chairDropdown.gameObject.SetActive(true);
        tableDropdown.gameObject.SetActive(true);
        speedDropdown.gameObject.SetActive(true);
        floorDropdown.gameObject.SetActive(true);
        loadDropdown.gameObject.SetActive(true);
        algorithmDropdown.gameObject.SetActive(true);
        tableDropdown.gameObject.SetActive(true);
        chairDropdown.gameObject.SetActive(true);
        IDField.gameObject.SetActive(true);
        menuButton.gameObject.SetActive(true);
        stopButton.gameObject.SetActive(false);
        this.gameObject.GetComponent<Image>().color = new Color(0.247f, 0.247f, 0.247f, 1.0f);
    }

    public void stopAction() {
        if (hasStarted) {
            hasStarted = false;
            showUI();
        }

    }

    void startAction() {
        int floorcount = cameraobj.GetComponent<ObjectPlacement>().getFloorCount();

        if (floorcount >= 200 && floorcount <= 8000) {
            hasStarted = true;
            hideUI();
        }
    }

    void exitAction() {
        show = true;
    }

    void menuAction() {
        SceneManager.LoadScene(sceneName: "MenuScene");
    }

    void chairAction() {
        inactiveColor();
        chairButton.GetComponent<Image>().color = Color.green;
    }

    void tableAction() {
        inactiveColor();
        tableButton.GetComponent<Image>().color = Color.green;
    }

    void chestAction() {
        inactiveColor();
        chestButton.GetComponent<Image>().color = Color.green;
    }

    void floorAction() {
        inactiveColor();
        floorButton.GetComponent<Image>().color = Color.green;
    }

    void wallAction() {
        inactiveColor();
        wallButton.GetComponent<Image>().color = Color.green;
    }

    void rovacAction() {
        inactiveColor();
        rovacButton.GetComponent<Image>().color = Color.green;
    }

    void deleteAction() {
        inactiveColor();
        if (bulkActive) {
            bulkActive = false;
        }
        bulkButton.GetComponent<Image>().color = Color.white;
        deleteButton.GetComponent<Image>().color = Color.Lerp(Color.white, Color.red, 0.5f);
    }
    void bulkAction() {
        if (bulkActive) {
            bulkActive = false;
            bulkButton.GetComponent<Image>().color = Color.white;
        } else {
            bulkActive = true;
            bulkButton.GetComponent<Image>().color = Color.Lerp(Color.white, Color.yellow, 0.5f);
        }

        deleteButton.GetComponent<Image>().color = Color.white;
    }

    void inactiveColor() {
        chairButton.GetComponent<Image>().color = Color.white;
        tableButton.GetComponent<Image>().color = Color.white;
        chestButton.GetComponent<Image>().color = Color.white;
        wallButton.GetComponent<Image>().color = Color.white;
        floorButton.GetComponent<Image>().color = Color.white;
        rovacButton.GetComponent<Image>().color = Color.white;
        deleteButton.GetComponent<Image>().color = Color.white;
    }

    // handles the change in floor dropdown 
    void SpeedValueChanged(TMP_Dropdown change) {
        int speedChoice = change.value;
        switchSimulationSpeed(speedChoice);
    }

    // Will change the speed of the simulation when selected by the user
    void switchSimulationSpeed(int choice) {
        switch (choice) {
            case 0:
                simSpeed = 1;
                break;
            case 1:
                simSpeed = 50;
                break;
            case 2:
                simSpeed = 100;
                break;
            default:
                break;
        }
    }

    public int getSimSpeed() {
        return simSpeed;
    }
}