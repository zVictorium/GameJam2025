using UnityEngine;
using TMPro;

public class LevelFinder : MonoBehaviour
{
    private TMP_Text levelText;

    void Start()
    {
        levelText = GetComponent<TMP_Text>();
        if (levelText == null)
        {
            Debug.LogError("El componente TMP_Text debe estar en el mismo objeto que LevelFinder");
            return;
        }
        UpdateLevelText();
    }

    void UpdateLevelText()
    {
        int currentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        levelText.text = "Level " + (currentLevel - 1);
    }
}
