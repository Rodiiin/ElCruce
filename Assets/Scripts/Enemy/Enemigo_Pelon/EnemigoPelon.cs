using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemigoPelon : MonoBehaviour
{
    
    [Header("Configuración de Movimiento")]
    public float velocidad = 2f;
    public float distanciaPatrulla = 3f;
    public float tiempoEsperaIdle = 1.5f;

    [Header("Configuración de Ataque")]
    public float rangoAtaque = 1.5f;     // Qué tan cerca debe estar el player
    public LayerMask capaJugador;        // Selecciona "Player" en el Inspector
    public GameObject efectoHumo;        // Arrastra aquí el hijo EfectoHumoGolpe
    public float cooldownAtaque = 2f;    // Tiempo entre golpes
    private bool puedeAtacar = true;
    private bool atacando = false;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector3 posicionInicial;
    private bool moviendoADerecha = true;
    private bool estaEsperando = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        posicionInicial = transform.position;

        if(efectoHumo != null) efectoHumo.SetActive(false);
    }

    void Update()
    {
        if (estaEsperando || atacando) return;

        // Calcular límites
        float limiteDerecho = posicionInicial.x + distanciaPatrulla;
        float limiteIzquierdo = posicionInicial.x - distanciaPatrulla;

        // Lógica de cambio de dirección
        if (moviendoADerecha && transform.position.x >= limiteDerecho)
        {
            StartCoroutine(PausaEnIdle(false));
        }
        else if (!moviendoADerecha && transform.position.x <= limiteIzquierdo)
        {
            StartCoroutine(PausaEnIdle(true));
        }
    }

    void FixedUpdate()
    {

        // 1. DETECCIÓN DE JUGADOR
        Collider2D jugadorDetectado = Physics2D.OverlapCircle(transform.position, rangoAtaque, capaJugador);

        if (jugadorDetectado != null && puedeAtacar)
        {
            // Solo atacamos si el jugador NO está muerto (opcional pero recomendado)
            VidaJugador v = jugadorDetectado.GetComponent<VidaJugador>();
            if (v != null && v.vidasActuales > 0)
            {
                StartCoroutine(SecuenciaAtaque(jugadorDetectado.gameObject));
            }
        }


        // 2. LÓGICA DE MOVIMIENTO FÍSICO
        if (!estaEsperando && !atacando)
        {
            float direccion = moviendoADerecha ? 1 : -1;
            rb.velocity = new Vector2(direccion * velocidad, rb.velocity.y);
            
            animator.SetBool("isWalking", true);
            animator.SetFloat("DirectionX", direccion); 
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            animator.SetBool("isWalking", false);
        }
    }

    IEnumerator SecuenciaAtaque(GameObject jugador)
    {
        puedeAtacar = false;
        atacando = true; // Bloquea el movimiento de patrulla
        
        // --- EFECTO VISUAL DEL HUMO ---
        if (efectoHumo != null)
        {
            // Orientamos el humo
            SpriteRenderer humoSR = efectoHumo.GetComponent<SpriteRenderer>();
            if (humoSR != null) humoSR.flipX = !moviendoADerecha;

            // Movemos el humo un poco hacia adelante del enemigo para que no salga de su centro
            float offsetHumo = moviendoADerecha ? 0.5f : -0.5f;
            efectoHumo.transform.localPosition = new Vector3(offsetHumo, 0, 0);

            efectoHumo.SetActive(true);
            
            Animator humoAnim = efectoHumo.GetComponent<Animator>();
            if (humoAnim != null) 
            {
                humoAnim.ResetTrigger("Humo_Ataque"); 
                humoAnim.SetTrigger("Humo_Ataque");
            }
        }

        // --- DAÑO AL JUGADOR ---
        VidaJugador vidaPlayer = jugador.GetComponent<VidaJugador>();
        if (vidaPlayer != null)
        {
            // Calculamos la dirección del golpe para el Knockback
            // (Posición Jugador - Posición Enemigo) nos da el vector hacia afuera
            Vector2 direccionGolpe = jugador.transform.position - transform.position;
            
            // Si el jugador está muy encima, forzamos un poco de elevación (0.5f en Y)
            direccionGolpe.y += 0.5f; 

            vidaPlayer.RecibirDaño(direccionGolpe);
        }

        yield return new WaitForSeconds(1f); // Tiempo que se queda parado "golpeando"
        
        if (efectoHumo != null) efectoHumo.SetActive(false);
        
        atacando = false; // Permite volver a patrullar o esperar
        
        yield return new WaitForSeconds(cooldownAtaque);
        puedeAtacar = true;
    }

    IEnumerator PausaEnIdle(bool nuevaDireccion)
    {
        estaEsperando = true;
        animator.SetBool("isWalking", false);

        yield return new WaitForSeconds(tiempoEsperaIdle);
        moviendoADerecha = nuevaDireccion;
        estaEsperando = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}
