using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenuUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private bool isPaused = false;
    void Start() {
        Time.timeScale = 1f; // El tiempo corre normalmente al iniciar
        isPaused = false; // Asegúrate de que no esté pausado al principio
        PauseMenuUI.SetActive(false);
    }
    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(isPaused) {
                resume();
            }
            else {
                pause();
            }
        }
    }
    public void pause() {
        isPaused = true;
        Time.timeScale = 0f; // pausa el juego
        PauseMenuUI.SetActive(true);
    }

    public void resume() {
        isPaused = false;
        Time.timeScale = 1f; // reanuda el juego
        PauseMenuUI.SetActive(false);
    }

    public void restart() {
        isPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void loadMenu() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

}   
