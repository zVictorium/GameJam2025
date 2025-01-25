using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject PauseButton;
    [SerializeField] private GameObject PauseMenuUI;
    [SerializeField] private GameObject blurBackground;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(Time.timeScale == 1) {
                pause();
            }
            else {
                resume();
            }
        }
    }
    public void pause() {
        Time.timeScale = 0f; // pausa el juego
        PauseButton.SetActive(false);
        PauseMenuUI.SetActive(true);
        blurBackground.SetActive(true);
    }

    public void resume() {
        Time.timeScale = 1f; // reanuda el juego
        PauseButton.SetActive(true);
        PauseMenuUI.SetActive(false);
        blurBackground.SetActive(false);
    }

}   
