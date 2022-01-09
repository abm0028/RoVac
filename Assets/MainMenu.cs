using UnityEngine;
using UnityEngine.SceneManagement;
public class SplashMen : MonoBehaviour
{

    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
