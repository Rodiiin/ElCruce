using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoJugador2 : MonoBehaviour
{
    [Header("Configuración movimiento")]
    public float velocidad = 5f;
    public float suavizado = 0.05f;

    [Header("Configuración Salto")]
    public float fuerzaSalto = 5f;
    public float fuerzaSegundoSalto = 5f;
    public Transform detectorSuelo;
    public float radioDeteccion = 0.1f;
    public LayerMask capaSuelo;
    public int saltosMaximos = 2;
    private int saltosRealizados;

    [Header("Configuración Dash/Ataque")]
    public float velocidadDash = 20f;
    public float duracionDash = 0.2f;
    public float cooldownDash = 1f;
    public float tiempoAtaque = 0.3f;
    public float cooldownAtaque = 0.2f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private VidaJugador2 vida; 
    
    private float mover;
    private Vector2 velocidadSuavizada = Vector2.zero;
    private bool estaEnSuelo;
    private bool estaHaciendoDash = false;
    private bool dashDisponible = true;
    private bool atacando = false;
    private bool ataqueDisponible = true;
    private float gravedadOriginal;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        vida = GetComponent<VidaJugador2>();
        gravedadOriginal = rb.gravityScale;
    }

    void Update()
    {
        if (vida != null && (vida.estaMuerto || vida.recibiendoDaño)) 
        {
            mover = 0; // Reseteamos mover para que no se deslice
            return;
        }

        if (estaHaciendoDash || atacando) return;

        // ---- TECLA DASH: H ----
        if (Input.GetKeyDown(KeyCode.H) && dashDisponible)
             StartCoroutine(RealizarDash());
        
        // ---- TECLA ATAQUE: N ----
        if (Input.GetKeyDown(KeyCode.N) && ataqueDisponible)
            StartCoroutine(RealizarAtaque());
        

        // ---- MOVIMIENTO: B (izquierda) y M (derecha) ----
        mover = 0f;
        if (Input.GetKey(KeyCode.M)) mover = 1f;
        if (Input.GetKey(KeyCode.B)) mover = -1f;

        // --- Lógica de giro
        if (mover > 0) spriteRenderer.flipX = true;
        else if (mover < 0) spriteRenderer.flipX = false;


        if (estaEnSuelo && rb.velocity.y <= 0.1f)
            saltosRealizados = 0;
        

        // ---- TECLA SALTO: J ----
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (estaEnSuelo || saltosRealizados < saltosMaximos)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);

                // Aplicamos fuerza según el número de salto
                float fuerzaAAplicar = (saltosRealizados == 0) ? fuerzaSalto : fuerzaSegundoSalto;
                rb.AddForce(Vector2.up * fuerzaAAplicar, ForceMode2D.Impulse);
                saltosRealizados++;
                if (animator != null) animator.SetTrigger("Jump");
            }
        }

        // --- ACTUALIZAR PARÁMETROS DE ANIMACIÓN ---
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(mover));
            animator.SetFloat("VelY", rb.velocity.y);
            animator.SetBool("isGrounded", estaEnSuelo);
        }
    }

    void FixedUpdate()
    {

        estaEnSuelo = Physics2D.OverlapCircle(detectorSuelo.position, radioDeteccion, capaSuelo);
        
        if (estaHaciendoDash) return;

        Vector2 velocidadObjetivo = new Vector2(mover * velocidad, rb.velocity.y);
        //rb.velocity = Vector2.SmoothDamp(rb.velocity, velocidadObjetivo, ref velocidadSuavizada, suavizado);
        float nuevaVelX = Mathf.Lerp(rb.velocity.x, mover * velocidad, suavizado);
        rb.velocity = new Vector2(nuevaVelX, rb.velocity.y);

    }

    private IEnumerator RealizarDash()
    {
        dashDisponible = false;
        estaHaciendoDash = true;

        if (animator != null) animator.SetTrigger("Dash");

        rb.gravityScale = 0f;

        float direccionDash = spriteRenderer.flipX ? 1f : -1f;
        rb.velocity = new Vector2(direccionDash * velocidadDash, 0f);

        yield return new WaitForSeconds(duracionDash);

        rb.gravityScale = gravedadOriginal;
        estaHaciendoDash = false;

        yield return new WaitForSeconds(cooldownDash);
        dashDisponible = true;
    }

    private IEnumerator RealizarAtaque()
    {
        ataqueDisponible = false;
        atacando = true;

        if (animator != null) animator.SetTrigger("Attack");

        yield return new WaitForSeconds(tiempoAtaque);

        atacando = false;

        yield return new WaitForSeconds(cooldownAtaque);
        ataqueDisponible = true;
    }

    private void OnDrawGizmos()
    {
        if (detectorSuelo != null)
        {
            Gizmos.color = Color.blue; // Azul para distinguirlo del jugador 1
            Gizmos.DrawWireSphere(detectorSuelo.position, radioDeteccion);
        }
    }
}