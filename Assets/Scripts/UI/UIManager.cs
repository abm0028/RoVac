using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {
    public Button chairButton, tableButton, chestButton, floorButton, wallButton, rovacButton, saveButton, loadButton, deleteButton, bulkButton, exitButton, stopButton, startButton, menuButton;
    public TMP_Dropdown chairDropdown, tableDropdown, speedDropdown, floorDropdown, algorithmDropdown;
    public TMP_InputField IDField;
    bool bulkActive = false;
    bool hasStarted = false;
    ObjectPlacement objectscript;
    public Camera cameraobj;

    // Start is called before the first frame update
    void Start() {

        chairButton.GetComponent<Button>().onClick.AddListener(chairAction);
        tableButton.GetComponent<Button>().onClick.AddListener(tableAction);
        chestButton.GetComponent<Button>().onClick.AddListener(chestAction);
        floorButton.GetComponent<Button>().onClick.AddListener(floorAction);
        wallButton.GetComponent<Button>().onClick.AddListener(wallAction);
        rovacButton.GetComponent<Button>().onClick.AddListener(rovacAction);
        saveButton.GetComponent<Button>().onClick.AddListener(saveAction);
        loadButton.GetComponent<Button>().onClick.AddListener(loadAction);
        deleteButton.GetComponent<Button>().onClick.AddListener(deleteAction);
        bulkButton.GetComponent<Button>().onClick.AddListener(bulkAction);
        exitButton.GetComponent<Button>().onClick.AddListener(exitAction);
        menuButton.GetComponent<Button>().onClick.AddListener(menuAction);
        stopButton.GetComponent<Button>().onClick.AddListener(stopAction);
        startButton.GetComponent<Button>().onClick.AddListener(startAction);

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
        algorithmDropdown.gameObject.SetActive(false);
        tableDropdown.gameObject.SetActive(false);
        chairDropdown.gameObject.SetActive(false);
        IDField.gameObject.SetActive(false);
        menuButton.gameObject.SetActive(false);
        stopButton.gameObject.SetActive(true);

        this.gameObject.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    }

    void showUI() {
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
        Application.Quit();
    }

    void menuAction() {
        SceneManager.LoadScene(sceneName: "MenuScene");
    }

    void saveAction() {

    }

    void loadAction() {

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
        }
        else {
            bulkActive = true;
            bulkButton.GetComponent<Image>().color = Color.yellow;
            chairButton.GetComponent<Image>().color = Color.gray;
            tableButton.GetComponent<Image>().color = Color.gray;
            rovacButton.GetComponent<Image>().color = Color.gray;
            deleteButton.GetComponent<Image>().color = Color.gray;
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
}

