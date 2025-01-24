using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    private Vector2 direction = Vector2.zero;
    private bool isMoving = false;
    private float speed = 7.5f; // Reducida de 5f a 3f
    private int size = 0; // Cambiado de 1 a 0
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool isInsideMap = false;
    private Vector2 targetPosition;
    private float stoppingLerpSpeed = 0.1f; // Nueva variable para controlar la suavidad de la parada
    private float currentScale = 0f; // Cambiado de 1f a 0f
    private float targetScale = 0f;  // Cambiado de 1f a 0f
    private float scalelinearVelocity = 0f;
    private float scaleDuration = 0.25f; // Duración de la transición en segundos
    private Vector2 initialPosition;
    private bool hitWall = false;
    private bool isExploding = false;

    private void Start()
    {
        spriteRenderer = transform.Find("Visuals").GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        targetPosition = rb.position;
        initialPosition = rb.position;
        transform.localScale = Vector3.zero; // Asegurar que empiece con escala 0
        SetSize(1); // Añadido al final de Start
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
            
            // Restaurar tamaño si llegamos a la posición inicial después de morir
            if (hitWall && targetPosition == initialPosition)
            {
                hitWall = false;
                SetSize(1);
            }
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
        if (isExploding) return; // No permitir movimiento mientras explota
        
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
            if (map.IsWall())
            {
                hitWall = true;
                SetSize(0); // Reducir tamaño a 0 al morir
                Stop();
                targetPosition = initialPosition;
            }
            else if (!map.IsWall())
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
        if (hitWall)
            spriteRenderer.color = Color.blue;
        else
            spriteRenderer.color = isInsideMap ? Color.red : Color.white;
    }

    private void UpdateScale()
    {
        if (currentScale != targetScale)
        {
            currentScale = Mathf.SmoothDamp(currentScale, targetScale, ref scalelinearVelocity, scaleDuration);
            transform.localScale = new Vector3(currentScale, currentScale, 1f);
            
            // Actualizar estado de explosión
            if (targetScale == 0)
            {
                isExploding = currentScale > 0.01f;
            }
        }
    }

    private void Stop()
    {
        if (!isMoving) return;
        
        isMoving = false;
        
        if (!hitWall)
        {
            // Calcular la posición objetivo redondeada solo si no golpeó una pared
            float roundedX = Mathf.Round(transform.position.x - 0.5f) + 0.5f;
            float roundedY = Mathf.Round(transform.position.y - 0.5f) + 0.5f;

            if (direction.x > 0) roundedX += GetOffsetFromSize(size);
            else if (direction.x < 0) roundedX -= GetOffsetFromSize(size);
            else if (direction.y > 0) roundedY += GetOffsetFromSize(size);
            else if (direction.y < 0) roundedY -= GetOffsetFromSize(size);

            targetPosition = new Vector2(roundedX, roundedY);
        }
        
        direction = Vector2.zero;
    }

    public void SetSize(int newSize)
    {
        size = newSize;
        targetScale = newSize;
        if (newSize == 0)
        {
            isExploding = true;
        }
    }

    static int GetOffsetFromSize(int tileSize)
    {
        if (tileSize == 1) return 1;
        if (tileSize == 3) return 2;
        if (tileSize == 5) return 3; 
        if (tileSize == 7) return 4; 
        return 5; 
    }
}