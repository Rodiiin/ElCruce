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
    public float fuerzaSegundoSalto = 0.001f;
    public Transform detectorSuelo;
    public float radioDeteccion = 0.9f;
    public LayerMask capaSuelo;
    public int saltosMaximos = 2;
    private int saltosRealizados;

    [Header("Configuración Daño")]
    public float fuerzaImpulso = 5f;
    public float duracionParpadeo = 0.5f;

    [Header("Configuración Vidas")]
    public int vidasMaximas = 3;
    private int vidasActuales;
    private bool estaMuerto = false;

    [Header("Configuración Dash")]
    public float velocidadDash = 20f;
    public float duracionDash = 0.2f;
    public float cooldownDash = 1f;
    private bool estaHaciendoDash = false;
    private bool dashDisponible = true;
    private float gravedadOriginal;

    [Header("Configuración Ataque")]
    public float tiempoAtaque = 0.3f;
    public float cooldownAtaque = 0.2f;
    private bool atacando = false;
    private bool ataqueDisponible = true;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float mover;
    private Vector2 velocidadSuavizada = Vector2.zero;
    private bool estaEnSuelo;
    private bool recibiendoDaño = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        vidasActuales = vidasMaximas;
        gravedadOriginal = rb.gravityScale;
    }

    void Update()
    {
        if (recibiendoDaño || estaMuerto || estaHaciendoDash || atacando) return;

        // ---- TECLA DASH: U ----
        if (Input.GetKeyDown(KeyCode.H) && dashDisponible)
        {
            StartCoroutine(RealizarDash());
        }

        // ---- TECLA ATAQUE: K ----
        if (Input.GetKeyDown(KeyCode.N) && ataqueDisponible)
        {
            StartCoroutine(RealizarAtaque());
        }

        // ---- MOVIMIENTO: J (izquierda) y L (derecha) ----
        mover = 0f;
        if (Input.GetKey(KeyCode.M)) mover = 1f;
        if (Input.GetKey(KeyCode.B)) mover = -1f;

        // --- Lógica de giro
        if (mover > 0)
        {
            if (spriteRenderer != null) spriteRenderer.flipX = true;
        }
        else if (mover < 0)
        {
            if (spriteRenderer != null) spriteRenderer.flipX = false;
        }

        if (estaEnSuelo)
        {
            saltosRealizados = 0;
        }

        // ---- TECLA SALTO: I ----
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (estaEnSuelo || saltosRealizados < saltosMaximos)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);

                if (saltosRealizados == 0 && estaEnSuelo)
                {
                    rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
                    saltosRealizados++;
                }
                else if(saltosRealizados < saltosMaximos   )
                {
                    rb.AddForce(Vector2.up * fuerzaSegundoSalto, ForceMode2D.Impulse);
                    saltosRealizados++;
                }

                

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

        if (rb == null || recibiendoDaño || estaMuerto || estaHaciendoDash) return;

        Vector2 velocidadObjetivo = new Vector2(mover * velocidad, rb.velocity.y);
        rb.velocity = Vector2.SmoothDamp(rb.velocity, velocidadObjetivo, ref velocidadSuavizada, suavizado);
    }

    public void RecibirDaño(Vector2 direccionGolpe)
    {
        if (recibiendoDaño || estaMuerto) return;

        vidasActuales--;

        if (vidasActuales <= 0)
        {
            Morir();
            return;
        }

        recibiendoDaño = true;

        if (animator != null) animator.SetTrigger("Hit");

        rb.velocity = Vector2.zero;
        rb.AddForce(direccionGolpe.normalized * fuerzaImpulso, ForceMode2D.Impulse);

        StartCoroutine(ParpadeoRojo());
    }

    private void Morir()
    {
        estaMuerto = true;
        recibiendoDaño = false;

        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            collider.size = new Vector2(collider.size.x, collider.size.y * 0.09f);
            collider.offset = new Vector2(collider.offset.x, collider.offset.y - (collider.size.y * .9f));
        }

        gameObject.layer = LayerMask.NameToLayer("DeadPlayer");

        if (animator != null) animator.SetTrigger("Death");

        this.enabled = false;

        Debug.Log("El jugador 2 ha muerto");
    }

    private IEnumerator ParpadeoRojo()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(duracionParpadeo);
        spriteRenderer.color = Color.white;
        recibiendoDaño = false;
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