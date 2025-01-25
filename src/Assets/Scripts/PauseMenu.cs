using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject PauseButton;
    [SerializeField] private GameObject PauseMenuUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private bool isPaused = false;

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(isPaused) {
                pause();
            }
            else {
                resume();
            }
        }
    }
    public void pause() {
        isPaused = true;
        Time.timeScale = 0f; // pausa el juego
        PauseButton.SetActive(false);
        PauseMenuUI.SetActive(true);
    }

    public void resume() {
        isPaused = false;
        Time.timeScale = 1f; // reanuda el juego
        PauseButton.SetActive(true);
        PauseMenuUI.SetActive(false);
    }

    public void restart() {
        isPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void loadMenu() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

}   
