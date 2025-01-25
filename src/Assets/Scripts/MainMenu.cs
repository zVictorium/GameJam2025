using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject MainMenuUI;
    [SerializeField] private GameObject SettingsMenuUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start() {
        MainMenuUI.SetActive(true);
        SettingsMenuUI.SetActive(false);
    }

    public void OpenSettings() {
        MainMenuUI.SetActive(false);
        SettingsMenuUI.SetActive(true);
    }
    public void PlayGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // Carga la siguiente escena
    }

    public void ExitGame() {
        Debug.Log("Exit...");
        Application.Quit(); // cierra aplicacion
    }
}
