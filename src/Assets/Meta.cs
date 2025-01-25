using UnityEngine;
using UnityEngine.SceneManagement;

public class Meta : MonoBehaviour
{
    [SerializeField] private Sprite activatedSprite;
    
    private Point[] points;
    private SpriteRenderer spriteRenderer;
    private bool isActivated = false;
    
    private void Start()
    {
        points = FindObjectsByType<Point>(FindObjectsSortMode.None);
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("No se encontró SpriteRenderer en Meta");
        }
        if (activatedSprite == null)
        {
            Debug.LogError("No se ha asignado el sprite activado en Meta");
        }
    }

    private void Update()
    {
        CheckAllPointsHidden();
    }

    private void CheckAllPointsHidden()
    {
        bool allHidden = true;
        foreach (Point point in points)
        {
            if (point.IsVisible())
            {
                allHidden = false;
                break;
            }
        }

        if (allHidden && !isActivated && spriteRenderer != null)
        {
            isActivated = true;
            spriteRenderer.sprite = activatedSprite;
        }
    }

    public bool IsActive()
    {
        return isActivated;
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
