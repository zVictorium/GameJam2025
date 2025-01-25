using UnityEngine;
using UnityEngine.SceneManagement;

public class Meta : MonoBehaviour
{
    [SerializeField] private Sprite activatedSprite;
    
    private Point[] points;
    private SpriteRenderer spriteRenderer;
    private Collider2D _collider2D;
    private bool isActivated = false;
    
    private void Start()
    {
        points = FindObjectsByType<Point>(FindObjectsSortMode.None);
        spriteRenderer = GetComponent<SpriteRenderer>();
        _collider2D = GetComponent<Collider2D>();
        
        if (spriteRenderer == null)
        {
            Debug.LogError("No se encontró SpriteRenderer en Meta");
        }
        if (activatedSprite == null)
        {
            Debug.LogError("No se ha asignado el sprite activado en Meta");
        }
        if (_collider2D == null)
        {
            Debug.LogError("No se encontró Collider2D en Meta");
        }
        else
        {
            _collider2D.isTrigger = false;
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
            if (_collider2D != null)
            {
                _collider2D.isTrigger = true;
            }
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
