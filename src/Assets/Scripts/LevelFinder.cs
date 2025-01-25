using UnityEngine;
using TMPro; // Asegúrate de importar TextMeshPro

public class LevelFinder : MonoBehaviour
{
    public TMP_Text levelText; // Referencia al componente TextMeshPro

    void Start()
    {
        UpdateLevelText();
    }

    void UpdateLevelText()
    {
        // Obtén el índice de la escena activa
        int currentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

        // Actualiza el texto del TMP_Text con el nivel
        if (levelText != null)
        {
            levelText.text = "Level: " + (currentLevel - 1);
        }
        else
        {
            Debug.LogWarning("No se ha asignado el TMP_Text al script LevelFinder.");
        }
    }
}
