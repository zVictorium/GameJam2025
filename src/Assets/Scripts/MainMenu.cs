using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PlayGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2); // Carga el nivel 1
    }

    public void LevelSelector() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // Carga el mapa de niveles
    }

    public void ExitGame() {
        Debug.Log("Exit...");
        Application.Quit(); // cierra aplicacion
    }
}
