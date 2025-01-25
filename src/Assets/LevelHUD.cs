using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelHUD : MonoBehaviour
{
    private TextMeshProUGUI levelText;

    void Start()
    {
        levelText = GetComponent<TextMeshProUGUI>();
        if (levelText == null)
        {
            Debug.LogError("TextMeshProUGUI component not found on the same object as LevelHUD!");
            return;
        }

        int levelNumber = SceneManager.GetActiveScene().buildIndex - 1;
        levelText.text = $"Level {levelNumber}";
    }
}
