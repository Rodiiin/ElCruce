using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoUnico : MonoBehaviour
{
    [Header("Configuración movimiento")]
    public float velocidad = 5f;
    public float suavizado = 0.05f;

    [Header("Configuración Salto")]
    public float fuerzaSalto = 5f;
    public Transform detectorSuelo; // Un objeto vacío en los pies del personaje
    public float radioDeteccion = 0.2f;
    public LayerMask capaSuelo; // Selecciona la capa del suelo en el inspector

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float mover;
    private Vector2 velocidadSuavizada = Vector2.zero;
    private bool estaEnSuelo;


    void Start()
    {
        // Buscamos los componentes en ESTE mismo objeto
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

  

    }

    // Update is called once per frame
    void Update()
    {
        // Detectar entrada
        mover = Input.GetAxis("Horizontal");

        // --- Lógica de giro
        if (mover > 0) {
            // Mirar a la izquierda
            if (spriteRenderer != null) spriteRenderer.flipX = true;
        }
        else if (mover < 0) {
            // Mirar a la derehca
            if (spriteRenderer != null) spriteRenderer.flipX = false;
        }

        // - LÓGICA DE SALTO (Detección) ---
        // Verificamos si tocamos el suelo
        estaEnSuelo = Physics2D.OverlapCircle(detectorSuelo.position, radioDeteccion, capaSuelo);

        if (Input.GetKeyDown(KeyCode.W) && estaEnSuelo)
        {
            rb.velocity = new Vector2(rb.velocity.x, fuerzaSalto);
            if (animator != null) animator.SetTrigger("Jump"); 
        }

        // --- ACTUALIZAR PARÁMETROS DE ANIMACIÓN ---
        if (animator != null) {
            // Speed para caminar
            animator.SetFloat("Speed", Mathf.Abs(mover));
            // VelY para saber si sube o baja
            animator.SetFloat("VelY", rb.velocity.y);
            // Bool para saber si está en el suelo
            animator.SetBool("isGrounded", estaEnSuelo);
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // Aplicamos el movimiento físico
        Vector2 velocidadObjetivo = new Vector2(mover * velocidad, rb.velocity.y);
        
        // Detener el movimiento inmediatamente al soltar
        rb.velocity = Vector2.SmoothDamp(rb.velocity, velocidadObjetivo, ref velocidadSuavizada, suavizado);
        
    }

    // --- VISUALIZACIÓN DE GIZMOS ---
    private void OnDrawGizmos()
    {
        if (detectorSuelo != null)
        {
            // Dibujamos una esfera roja en la posición del detector para ver el rango
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(detectorSuelo.position, radioDeteccion);
        }
    }
}
