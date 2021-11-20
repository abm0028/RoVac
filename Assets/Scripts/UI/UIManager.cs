using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
    public Button chairButton, tableButton, chestButton, floorButton, wallButton, rovacButton, saveButton, loadButton;
    public TMP_Dropdown chairDropdown, tableDropdown, speedDropdown, floorDropdown, algorithmDropdown;

    bool chairActive = false;
    bool tableActive = false;
    bool chestActive = false;
    bool floorActive = false;
    bool wallActive = false;
    bool rovacActive = false;

    // Start is called before the first frame update
    void Start() {
        chairDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate {
            ChairValueChanged(chairDropdown);
        });

        tableDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate {
            TableValueChanged(tableDropdown);
        });

        speedDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate {
            SpeedValueChanged(speedDropdown);
        });

        algorithmDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate {
            AlgorithmValueChanged(algorithmDropdown);
        });

        floorDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate {
            FloorValueChanged(floorDropdown);
        });

        chairButton.GetComponent<Button>().onClick.AddListener(chairAction);
        tableButton.GetComponent<Button>().onClick.AddListener(tableAction);
        chestButton.GetComponent<Button>().onClick.AddListener(chestAction);
        floorButton.GetComponent<Button>().onClick.AddListener(floorAction);
        wallButton.GetComponent<Button>().onClick.AddListener(wallAction);
        rovacButton.GetComponent<Button>().onClick.AddListener(rovacAction);
        saveButton.GetComponent<Button>().onClick.AddListener(saveAction);
        loadButton.GetComponent<Button>().onClick.AddListener(loadAction);

    }

    void saveAction() {
        // Debug.Log("save");
    }

    void loadAction() {
        // Debug.Log("load");
    }
    void FloorValueChanged(TMP_Dropdown change) {

    }

    void AlgorithmValueChanged(TMP_Dropdown change) {

    }
    void SpeedValueChanged(TMP_Dropdown change) {

    }

    void TableValueChanged(TMP_Dropdown change) {

    }
    // Update is called once per frame
    void Update() {

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

    void resetButtons() {
        tableActive = false;
        chestActive = false;
        chairActive = false;
        floorActive = false;
        wallActive = false;
        rovacActive = false;
    }

    void ChairValueChanged(TMP_Dropdown change) {

    }

    void inactiveColor() {
        chairButton.GetComponent<Image>().color = Color.white;
        tableButton.GetComponent<Image>().color = Color.white;
        chestButton.GetComponent<Image>().color = Color.white;
        wallButton.GetComponent<Image>().color = Color.white;
        floorButton.GetComponent<Image>().color = Color.white;
        rovacButton.GetComponent<Image>().color = Color.white;
    }
}

