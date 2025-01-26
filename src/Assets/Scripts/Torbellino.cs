using UnityEngine;

public class Torbellino : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Collider2D col2D;
    private Vector3 originalScale;
    private bool isDisabling = false;
    private float fadeSpeed = 2f;
    private float scaleReduction = 0.7f;

    void Start()
    {
        spriteRenderer = transform.Find("Visuals").GetComponent<SpriteRenderer>();
        col2D = GetComponent<Collider2D>();
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (isDisabling)
        {
            Color currentColor = spriteRenderer.color;
            currentColor.a = Mathf.MoveTowards(currentColor.a, 0f, fadeSpeed * Time.deltaTime);
            spriteRenderer.color = currentColor;

            transform.localScale = Vector3.Lerp(
                transform.localScale, 
                originalScale * scaleReduction, 
                fadeSpeed * Time.deltaTime
            );

            if (currentColor.a <= 0)
            {
                isDisabling = false;
                spriteRenderer.enabled = false;
                col2D.enabled = false;
            }
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void DisableVisuals()
    {
        isDisabling = true;
    }

    public void EnableVisuals()
    {
        isDisabling = false;
        transform.localScale = originalScale;
        Color color = spriteRenderer.color;
        color.a = 1f;
        spriteRenderer.color = color;
        spriteRenderer.enabled = true;
        col2D.enabled = true;
    }
}
