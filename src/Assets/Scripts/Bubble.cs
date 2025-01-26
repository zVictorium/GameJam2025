using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class Bubble : MonoBehaviour
{
    [SerializeField] private GameObject transitionCanvas; // Añadir esta línea al inicio de la clase
    private Vientos vientosEffect; // Eliminar SerializeField
    private AudioSource efectoMetaAudio; // Nueva variable
    private AudioSource efectoLlaveAudio; // Nueva variable

    private const float SPEED = 15.0f;
    private const float STOPPING_LERP_SPEED = 0.5f;
    private const float SCALE_DURATION = 0.25f;
    private const float POSITION_THRESHOLD = 0.01f;
    private const float MOVEMENT_INTERPOLATION = 0.8f;
    private const float TORBELLINO_LERP_SPEED = 10f;

    // Reemplazamos platformTilemap por torbellinos
    private Torbellino[] allTorbellinos;

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
    private bool isMovingToTorbellino = false;
    private Torbellino currentTorbellino;

    // Estado de tamaño
    private int size;
    private float currentScale;
    private float targetScale;
    private float scaleVelocity;
    private bool isShrinking;

    // Estado del juego
    private bool isPlayingDeathAnimation;
    private Point[] allPoints;
    private Key[] allKeys;
    private int totalPoints;
    private int collectedPoints;

    private void Start()
    {
        InitializeComponents();
        InitializePhysics();
        InitializeState();
    }

    private void InitializeComponents()
    {
        // Intentar obtener el SpriteRenderer del objeto actual si no se encuentra en Visuals
        var visualsTransform = transform.Find("Visuals");
        if (visualsTransform != null)
        {
            spriteRenderer = visualsTransform.GetComponent<SpriteRenderer>();
        }
        
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        if (spriteRenderer == null)
        {
            Debug.LogError("No se pudo encontrar el SpriteRenderer en Bubble ni en sus hijos");
        }
        
        // Buscar el componente Vientos en la escena
        vientosEffect = FindFirstObjectByType<Vientos>();
        if (vientosEffect == null)
        {
            Debug.LogError("No se encontró el componente Vientos en la escena");
        }
        
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        allPoints = FindObjectsByType<Point>(FindObjectsSortMode.None);
        allKeys = FindObjectsByType<Key>(FindObjectsSortMode.None);
        allTorbellinos = FindObjectsByType<Torbellino>(FindObjectsSortMode.None);

        // Buscar el Audio Source del efecto meta
        GameObject efectoMeta = GameObject.Find("Efecto meta");
        if (efectoMeta != null)
        {
            efectoMetaAudio = efectoMeta.GetComponent<AudioSource>();
        }
        else
        {
            Debug.LogWarning("No se encontró el objeto 'Efecto meta'");
        }

        // Buscar el Audio Source del efecto llave
        GameObject efectoLlave = GameObject.Find("Efecto llave");
        if (efectoLlave != null)
        {
            efectoLlaveAudio = efectoLlave.GetComponent<AudioSource>();
        }
        else
        {
            Debug.LogWarning("No se encontró el objeto 'Efecto llave'");
        }
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
        totalPoints = allPoints.Length;
        collectedPoints = 0;
    }

    private void FixedUpdate()
    {
        if (isPlayingDeathAnimation) return;
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        if (isMovingToTorbellino)
        {
            vientosEffect.SetVisibility(true);
            // Movimiento suave hacia el torbellino o meta
            Vector2 currentPos = transform.position;
            Vector2 newPos = Vector2.Lerp(currentPos, targetPosition, Time.deltaTime * TORBELLINO_LERP_SPEED);
            rb.MovePosition(newPos);
            
            // Si estamos lo suficientemente cerca, finalizamos el movimiento
            if (Vector2.Distance(currentPos, targetPosition) < 0.01f)
            {
                isMovingToTorbellino = false;
                transform.position = targetPosition;
                rb.position = targetPosition;
                if (currentTorbellino != null)
                {
                    currentTorbellino.DisableVisuals();
                    currentTorbellino = null;
                }
            }
            return;
        }

        if (isMoving)
        {
            vientosEffect.SetVisibility(true);
            // Calcular el ángulo basado en la dirección
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            vientosEffect.RotateObject(angle - vientosEffect.transform.eulerAngles.z);
            MoveInDirection();
        }
        else
        {
            vientosEffect.SetVisibility(false);
            animator.SetBool("Horizontal", false);
            animator.SetBool("Vertical", false);
            
            if (ShouldSmoothStop())
            {
                SmoothStop();
            }
            else
            {
                CompleteStop();
            }
        }
    }

    private void MoveInDirection()
    {
        // Movemos la burbuja directamente sin alinear a la grid
        Vector2 newPos = rb.position + direction * SPEED * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
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

        // Activar los parámetros del animator según la dirección
        animator.SetBool("Horizontal", Mathf.Abs(newDirection.x) > 0);
        animator.SetBool("Vertical", Mathf.Abs(newDirection.y) > 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleMapCollision(other);
        HandlePointCollision(other);
        HandleMetaCollision(other);
        HandleKeyCollision(other);
    }

    private void HandleMapCollision(Collider2D other)
    {
        if (other.TryGetComponent<Torbellino>(out var torbellino))
        {
            targetPosition = torbellino.GetPosition();
            isMoving = false;
            isMovingToTorbellino = true;
            currentTorbellino = torbellino;
            direction = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Verificar si colisiona con Medusa
        if (other.TryGetComponent<Medusa>(out var medusa))
        {
            HandleWallCollision();
            return;
        }

        // Si es un TilemapCollider2D, tratarlo como pared
        if (other.GetComponent<TilemapCollider2D>() != null)
        {
            HandleWallCollision();
        }
    }

    private void HandleWallCollision()
    {
        hitWall = true;
        Stop();
        animator.SetTrigger("Death");
        animator.SetBool("Horizontal", false);
        animator.SetBool("Vertical", false);
        isPlayingDeathAnimation = true;
    }

    private void HandlePointCollision(Collider2D other)
    {
        if (!other.TryGetComponent<Point>(out var point)) return;
        if (isPlayingDeathAnimation) return; // Solo esta verificación es necesaria
        
        SetSize(size + 2);
        point.Hide();
        collectedPoints++;

        // Reproducir sonido cuando se recolecten todos los puntos
        if (collectedPoints == totalPoints && efectoMetaAudio != null)
        {
            efectoMetaAudio.Play();
        }
    }

    private void HandleMetaCollision(Collider2D other)
    {
        if (!other.TryGetComponent<Meta>(out var meta)) return;
        
        if (meta.IsActive())
        {
            SetSize(0); // Hacemos que la burbuja se encoja
            targetPosition = meta.transform.position;
            isMoving = false;
            isMovingToTorbellino = true;
            currentTorbellino = null;
            direction = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("Horizontal", false);
            animator.SetBool("Vertical", false);
            StartCoroutine(LoadNextLevelWithDelay(meta));
        }
        else
        {
            HandleWallCollision();
        }
    }
    
    private void HandleKeyCollision(Collider2D other)
    {
        if (!other.TryGetComponent<Key>(out var key)) return;
        if (isPlayingDeathAnimation) return;
        
        key.Hide();
        
        // Reproducir sonido al recoger la llave
        if (efectoLlaveAudio != null)
        {
            efectoLlaveAudio.Play();
        }
    }
    
    private IEnumerator LoadNextLevelWithDelay(Meta meta)
    {
        // Intentar obtener el canvas si no fue asignado
        if (transitionCanvas == null)
        {
            transitionCanvas = GameObject.Find("Canva");
        }

        // Si encontramos el canvas, reproducir la animación
        if (transitionCanvas != null)
        {
            Animator transition = transitionCanvas.GetComponent<Animator>();
            if (transition != null)
            {
                transition.SetTrigger("Start");
                yield return new WaitForSeconds(0.7f);
            }
        }
        else
        {
            Debug.LogWarning("Canvas de transición no encontrado - saltando animación");
            yield return new WaitForSeconds(0.1f);
        }
        
        meta.LoadNextLevel();
    }

    private void UpdateScale()
    {
        if (currentScale != targetScale)
        {
            currentScale = Mathf.SmoothDamp(currentScale, targetScale, ref scaleVelocity, SCALE_DURATION);
            transform.localScale = new Vector3(currentScale, currentScale, 1f);
            
            if (isShrinking && currentScale < 0.01f)
            {
                currentScale = 0f;
                transform.localScale = Vector3.zero;
                isShrinking = false;
            }
        }
    }

    private void Stop(bool removeStop = false)
    {
        if (!isMoving && !isMovingToTorbellino) return;
        
        isMoving = false;
        isMovingToTorbellino = false;
        vientosEffect.SetVisibility(false); // Ocultar vientos al detenerse
        
        // Desactivar los parámetros del animator
        animator.SetBool("Horizontal", false);
        animator.SetBool("Vertical", false);
        
        if (!hitWall)
        {
            rb.position = targetPosition;
            transform.position = targetPosition;
        }
        
        direction = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
    }

    public void SetSize(int newSize)
    {
        size = newSize;
        targetScale = newSize;
        if (newSize == 0)
        {
            isShrinking = true;
        }
    }

    static int GetOffsetFromSize(int tileSize)
    {
        // Simplificamos el offset para que sea más predecible
        return Mathf.Max(1, (tileSize + 1) / 2);
    }

    public void OnDeathAnimationComplete()
    {
        // Primero reseteamos el tamaño
        size = 0;
        currentScale = 0f;
        targetScale = 0f;
        transform.localScale = Vector3.zero;
        isShrinking = false;

        // Luego la posición
        ResetPosition();
        
        // Después el estado del juego
        ResetGameState();
        
        // Por último el nivel y establecemos el tamaño inicial
        ResetLevel();
        SetSize(1);
    }

    private void ResetGameState()
    {
        isPlayingDeathAnimation = false;
        hitWall = false;
        isMoving = false;
        direction = Vector2.zero;
        collectedPoints = 0; // Reseteamos los puntos recolectados
        animator.SetBool("Horizontal", false);
        animator.SetBool("Vertical", false);
    }

    private void ResetPosition()
    {
        rb.position = initialPosition;
        transform.position = initialPosition;
        targetPosition = initialPosition;
        
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    private void ResetLevel()
    {
        foreach (var torbellino in allTorbellinos)
        {
            torbellino.EnableVisuals();
        }

        foreach (Point point in allPoints)
        {
            point.Show();
        }

        foreach (Key key in allKeys)
        {
            key.Show();
        }

        // Encontrar y desactivar la meta
        Meta meta = FindFirstObjectByType<Meta>();
        if (meta != null)
        {
            meta.DeactivateMeta();
        }
    }

    // Nuevo método para obtener el conteo de puntos
    public (int collected, int total) GetPointCount()
    {
        return (collectedPoints, totalPoints);
    }
}