using UnityEngine;

public class Point : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;

    private void Start()
    {
        // Buscar el SpriteRenderer en el hijo "Visuals" si existe, si no en el objeto actual
        spriteRenderer = transform.Find("Visuals")?.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        
        circleCollider = GetComponent<CircleCollider2D>();

        // Verificar que tenemos los componentes necesarios
        if (spriteRenderer == null)
        {
            Debug.LogError("No se encontró SpriteRenderer en Point ni en sus hijos");
        }
        if (circleCollider == null)
        {
            Debug.LogError("No se encontró CircleCollider2D en Point");
        }
    }

    public void Hide()
    {
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (circleCollider != null) circleCollider.enabled = false;
    }

    public void Show()
    {
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (circleCollider != null) circleCollider.enabled = true;
    }

    public void Remove()
    {
        Hide();
    }

    public bool IsVisible()
    {
        return spriteRenderer != null && spriteRenderer.enabled;
    }
}
