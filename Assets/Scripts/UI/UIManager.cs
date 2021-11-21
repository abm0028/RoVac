using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
    public Button chairButton, tableButton, chestButton, floorButton, wallButton, rovacButton, saveButton, loadButton, deleteButton, bulkButton, exitButton;
    public TMP_Dropdown chairDropdown, tableDropdown, speedDropdown, floorDropdown, algorithmDropdown;
    bool bulkActive = false;

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

    }

    void exitAction() {
        Application.Quit();
    }

    void saveAction() {
        // Debug.Log("save");
    }

    void loadAction() {
        // Debug.Log("load");
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
        bulkButton.GetComponent<Image>().color = Color.white;
        deleteButton.GetComponent<Image>().color = Color.Lerp(Color.white, Color.red, 0.5f);
    }
    void bulkAction() {
        if (bulkActive) {
            bulkActive = false;
            bulkButton.GetComponent<Image>().color = Color.white;
        } else {
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
}

