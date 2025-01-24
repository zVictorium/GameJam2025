using UnityEngine;

public class Bubble : MonoBehaviour
{
    private Vector2 direction = Vector2.zero;
    private bool isMoving = false;
    private float speed = 5f; // Reducida de 5f a 3f
    private int size = 1; // Tiles que ocupa la burbuja
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool isInsideMap = false;
    private Vector2 targetPosition;
    private float stoppingLerpSpeed = 0.1f; // Nueva variable para controlar la suavidad de la parada
    private float currentScale = 1f;
    private float targetScale = 1f;
    private float scalelinearVelocity = 0f;
    private float scaleDuration = 0.25f; // Duración de la transición en segundos

    private void Start()
    {
        spriteRenderer = transform.Find("Visuals").GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        targetPosition = rb.position;
        SetSize(3);
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            Vector2 newPos = rb.position + direction * speed * Time.fixedDeltaTime;
            rb.MovePosition(Vector2.Lerp(rb.position, newPos, 0.8f));
        }
        else if (Vector2.Distance(rb.position, targetPosition) > 0.01f)
        {
            // Movimiento más suave al detenerse
            rb.MovePosition(Vector2.Lerp(rb.position, targetPosition, stoppingLerpSpeed));
            // Reducir gradualmente la velocidad
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, stoppingLerpSpeed);
        }
        else
        {
            // Asegurar que se detenga completamente
            rb.linearVelocity = Vector2.zero;
            rb.position = targetPosition;
        }
    }

    private void Update()
    {
        if (!isMoving)
        {
            CheckInput();
        }
        UpdateColor();
        UpdateScale();
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            SetDirection(Vector2.up);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            SetDirection(Vector2.down);
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            SetDirection(Vector2.left);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            SetDirection(Vector2.right);
    }

    private void SetDirection(Vector2 newDirection)
    {
        direction = newDirection;
        isMoving = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Map>(out var map))
        {
            isInsideMap = true;
            if (!map.IsWall())
            {
                Stop();
            }
        }
        else if (other.TryGetComponent<Point>(out var point))
        {
            SetSize(size + 2);
            point.Remove();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Map>(out var map))
        {
            isInsideMap = false;
        }
    }

    private void UpdateColor()
    {
        spriteRenderer.color = isInsideMap ? Color.red : Color.white;
    }

    private void UpdateScale()
    {
        if (currentScale != targetScale)
        {
            currentScale = Mathf.SmoothDamp(currentScale, targetScale, ref scalelinearVelocity, scaleDuration);
            transform.localScale = new Vector3(currentScale, currentScale, 1f);
        }
    }

    private void Stop()
    {
        if (!isMoving) return;
        
        isMoving = false;
        
        // Calcular la posición objetivo redondeada con el offset según la dirección
        float roundedX = Mathf.Round(transform.position.x);
        float roundedY = Mathf.Round(transform.position.y);

        // Ajustar el offset según la dirección
        if (direction.x > 0) {
            roundedX += GetOffsetFromSize(size);
            roundedX += 0.5f;
            roundedY += 0.5f;
        }
        if (direction.x < 0) {
            roundedX -= GetOffsetFromSize(size);
            roundedX -= 0.5f;
            roundedY += 0.5f;
        }
        if (direction.y > 0) {
            roundedX -= 0.5f;
            roundedY += GetOffsetFromSize(size);
            roundedY += 0.5f;
        }
        if (direction.y < 0) {
            roundedX -= 0.5f;
            roundedY -= GetOffsetFromSize(size);
            roundedY -= 0.5f;
        }

        targetPosition = new Vector2(roundedX, roundedY);
        direction = Vector2.zero;
        // Eliminamos el reseteo inmediato de velocidades para permitir una parada más suave
    }

    public void SetSize(int newSize)
    {
        size = newSize;
        targetScale = newSize;
    }

    static int GetOffsetFromSize(int tileSize)
    {
        if ((tileSize - 1) % 2 == 0 && tileSize >= 1)
            return (tileSize - 1) / 2;
        return 0;
    }
}