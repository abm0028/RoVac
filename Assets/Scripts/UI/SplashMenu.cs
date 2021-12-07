using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashMenu : MonoBehaviour {
    public Button exitButton, scoresButton;

    private Rect windowRect = new Rect((Screen.width - 200) / 2, (Screen.height - 300) / 2, 200, 75);

    // Only show it if needed.
    private bool show = false;

    public GUIStyle primaryButtonSkin;
    public GUIStyle secondaryButtonSkin;

    void Start() {
        exitButton.onClick.AddListener(exitAction);
    }

    public void Play() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // handles the exit button are you sure dialog box
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

    void exitAction() {
        show = true;
    }
}
