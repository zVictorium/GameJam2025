using UnityEngine;

public class Bubble : MonoBehaviour
{
    private const float SPEED = 15.0f;
    private const float STOPPING_LERP_SPEED = 0.75f;
    private const float SCALE_DURATION = 0.25f;
    private const float POSITION_THRESHOLD = 0.01f;
    private const float MOVEMENT_INTERPOLATION = 0.8f;

    [SerializeField] private Map platformTilemap;

    // Referencias a componentes
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Animator animator;

    // Estado de movimiento
    private Vector2 direction = Vector2.zero;
    private Vector2 targetPosition;
    private Vector2 initialPosition;
    private bool isMoving;
    private bool hitWall;

    // Estado de tamaño
    private int size;
    private float currentScale;
    private float targetScale;
    private float scaleVelocity;
    private bool isShrinking;

    // Estado del juego
    private bool isPlayingDeathAnimation;
    private Map currentMap;
    private Point[] allPoints;

    private void Start()
    {
        InitializeComponents();
        InitializePhysics();
        InitializeState();
    }

    private void InitializeComponents()
    {
        spriteRenderer = transform.Find("Visuals").GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        allPoints = FindObjectsByType<Point>(FindObjectsSortMode.None);
    }

    private void InitializePhysics()
    {
        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void InitializeState()
    {
        targetPosition = rb.position;
        initialPosition = rb.position;
        transform.localScale = Vector3.zero;
        SetSize(1);
    }

    private void FixedUpdate()
    {
        if (isPlayingDeathAnimation) return;
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        if (isMoving)
        {
            MoveInDirection();
        }
        else if (ShouldSmoothStop())
        {
            SmoothStop();
        }
        else
        {
            CompleteStop();
        }
    }

    private void MoveInDirection()
    {
        Vector2 newPos = rb.position + direction * SPEED * Time.fixedDeltaTime;
        rb.MovePosition(Vector2.Lerp(rb.position, newPos, MOVEMENT_INTERPOLATION));
    }

    private bool ShouldSmoothStop()
    {
        return Vector2.Distance(rb.position, targetPosition) > POSITION_THRESHOLD;
    }

    private void SmoothStop()
    {
        rb.MovePosition(Vector2.Lerp(rb.position, targetPosition, STOPPING_LERP_SPEED));
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, STOPPING_LERP_SPEED);
    }

    private void CompleteStop()
    {
        rb.linearVelocity = Vector2.zero;
        rb.position = targetPosition;
        
        if (hitWall && !isShrinking && targetPosition == initialPosition)
        {
            hitWall = false;
            SetSize(1);
        }
    }

    private void Update()
    {
        if (!isMoving) CheckInput();
        UpdateScale();
    }

    private bool IsCompletelyStill()
    {
        return !isMoving && 
               Vector2.Distance(rb.position, targetPosition) < POSITION_THRESHOLD && 
               rb.linearVelocity.magnitude == 0.0f;
    }

    private void CheckInput()
    {
        if (!IsCompletelyStill()) return;
        
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
        if (isShrinking || hitWall) return;
        direction = newDirection;
        isMoving = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleMapCollision(other);
        HandlePointCollision(other);
    }

    private void HandleMapCollision(Collider2D other)
    {
        if (!other.TryGetComponent<Map>(out var map)) return;

        currentMap = map;
        
        if (map.IsWall())
        {
            HandleWallCollision();
        }
        else
        {
            Stop(true);
        }
    }

    private void HandleWallCollision()
    {
        hitWall = true;
        Stop();
        animator.SetTrigger("Death");
        isPlayingDeathAnimation = true;
    }

    private void HandlePointCollision(Collider2D other)
    {
        if (!other.TryGetComponent<Point>(out var point)) return;
        if (isPlayingDeathAnimation) return; // Solo esta verificación es necesaria
        
        SetSize(size + 2);
        point.Hide();
    }

    private void UpdateScale()
    {
        if (currentScale != targetScale)
        {
            currentScale = Mathf.SmoothDamp(currentScale, targetScale, ref scaleVelocity, SCALE_DURATION);
            transform.localScale = new Vector3(currentScale, currentScale, 1f);
            
            Debug.Log($"UpdateScale -> size:{size}, currentScale:{currentScale}, targetScale:{targetScale}, isShrinking:{isShrinking}");
            
            if (isShrinking && currentScale < 0.01f)
            {
                currentScale = 0f;
                transform.localScale = Vector3.zero;
                isShrinking = false;
                Debug.Log("Shrinking complete!");
            }
        }
    }

    private void Stop(bool removeStop = false)
    {
        if (!isMoving) return;
        
        isMoving = false;
        
        if (!hitWall)
        {
            float roundedX = Mathf.Round(transform.position.x - 0.5f) + 0.5f;
            float roundedY = Mathf.Round(transform.position.y - 0.5f) + 0.5f;

            if (direction.x > 0) roundedX += GetOffsetFromSize(size);
            else if (direction.x < 0) roundedX -= GetOffsetFromSize(size);
            else if (direction.y > 0) roundedY += GetOffsetFromSize(size);
            else if (direction.y < 0) roundedY -= GetOffsetFromSize(size);

            targetPosition = new Vector2(roundedX, roundedY);

            if (removeStop && currentMap != null)
            {
                currentMap.DeleteTile(targetPosition);
            }
        }
        
        direction = Vector2.zero;
    }

    public void SetSize(int newSize)
    {
        Debug.Log($"SetSize called with newSize:{newSize} (previous size:{size})");
        size = newSize;
        targetScale = newSize;
        if (newSize == 0)
        {
            isShrinking = true;
        }
        Debug.Log($"After SetSize -> size:{size}, targetScale:{targetScale}, isShrinking:{isShrinking}");
    }

    static int GetOffsetFromSize(int tileSize)
    {
        if (tileSize == 1) return 1;
        if (tileSize == 3) return 2;
        if (tileSize == 5) return 3; 
        if (tileSize == 7) return 4; 
        return 5; 
    }

    public void OnDeathAnimationComplete()
    {
        Debug.Log("OnDeathAnimationComplete - Before reset -> " +
                  $"size:{size}, currentScale:{currentScale}, targetScale:{targetScale}");
        
        // Primero reseteamos el tamaño
        size = 0;
        currentScale = 0f;
        targetScale = 0f;
        transform.localScale = Vector3.zero;
        isShrinking = false;

        Debug.Log("After size reset -> " +
                  $"size:{size}, currentScale:{currentScale}, targetScale:{targetScale}");

        // Luego la posición
        ResetPosition();
        
        // Después el estado del juego
        ResetGameState();
        
        // Por último el nivel y establecemos el tamaño inicial
        ResetLevel();
        SetSize(1);
        
        Debug.Log("OnDeathAnimationComplete - Final state -> " +
                  $"size:{size}, currentScale:{currentScale}, targetScale:{targetScale}");
    }

    private void ResetGameState()
    {
        isPlayingDeathAnimation = false;
        hitWall = false;
        isMoving = false;
        direction = Vector2.zero;
        currentMap = null;
    }

    private void ResetPosition()
    {
        // Aseguramos que todas las posiciones están correctamente alineadas a la grilla
        float roundedX = Mathf.Round(initialPosition.x - 0.5f) + 0.5f;
        float roundedY = Mathf.Round(initialPosition.y - 0.5f) + 0.5f;
        Vector2 alignedPosition = new Vector2(roundedX, roundedY);
        
        rb.position = alignedPosition;
        transform.position = alignedPosition;
        targetPosition = alignedPosition;
        initialPosition = alignedPosition;
        
        // Detenemos cualquier velocidad residual
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.linearVelocity = Vector2.zero;
    }

    private void ResetLevel()
    {
        if (platformTilemap != null)
        {
            platformTilemap.ResetTilemap();
        }

        foreach (Point point in allPoints)
        {
            point.Show();
        }
    }
}