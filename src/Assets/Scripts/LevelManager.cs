using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour {
    public Button[] levelButtons; // Botones de los niveles

    void Start() {
        // Cargar el nivel desbloqueado más alto
        int levelReached = PlayerPrefs.GetInt("LevelReached", 1); // Por defecto, solo el nivel 1 está desbloqueado

        for (int i = 0; i < levelButtons.Length; i++) {
            if (i + 1 > levelReached) {
                levelButtons[i].interactable = false; // Bloquea los niveles aún no desbloqueados
            }
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void SelectLevel(int levelIndex) {
        SceneManager.LoadScene(levelIndex + 1); // Carga la escena del nivel seleccionado (escena 2 = nivel 1)
    }
    void CompleteLevel() {
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("LevelReached", currentLevel + 1); // Desbloquea el siguiente nivel
        PlayerPrefs.Save();
    }

}
