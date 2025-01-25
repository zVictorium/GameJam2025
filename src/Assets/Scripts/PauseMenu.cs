using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject PauseButton;
    [SerializeField] private GameObject PauseMenuUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void pause() {
        Time.timeScale = 0f; // pausa el juego
        PauseButton.SetActive(false);
        PauseMenuUI.SetActive(true);
    }

    public void resume() {
        Time.timeScale = 1f; // reanuda el juego
        PauseButton.SetActive(true);
        PauseMenuUI.SetActive(false);
    }
}   
