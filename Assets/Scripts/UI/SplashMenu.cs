using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashMenu : MonoBehaviour {
    public Button exitButton;

    void Start() {
        exitButton.onClick.AddListener(exitAction);
    }

    public void Play() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void exitAction() {
        Application.Quit();
    }
}
