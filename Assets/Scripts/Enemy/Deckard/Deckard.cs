using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deckard : MonoBehaviour
{
    [Header("Detección")]
    public Vector2 tamanoDeteccion = new Vector2(10f, 5f);
    public Vector2 offsetDeteccion = new Vector2(0f, 2.5f);
    public LayerMask capaJugadores;

    [Header("Zona de Huida")]
    public float radioHuida = 2f;        // Si el player entra aquí, Deckard huye
    public float velocidadHuida = 3f;    // Qué tan rápido huye
    public float distanciaMaxHuida = 6f; // Límite hasta donde puede huir

    [Header("Floor detection")]
    public Transform detectorSuelo;
    public float radioDeteccionSuelo = 0.1f;
    public LayerMask capaSuelo;
    private float gravedadOriginal;
    private Rigidbody2D rb;

    private Transform jugadorObjetivo;
    private Animator animator;
    private bool jugadorEnRango = false;
    private Vector3 posicionInicial;

    [Header("Ataque")]
    public GameObject navajaPrefab;
    public Transform puntoDisparo;
    public float tiempoEntreNavajas = 2f;
    private float cronometroAtaque;

    public GameObject efectoHumo;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        posicionInicial = transform.position; // Guardamos posición inicial

        if (rb != null)
            gravedadOriginal = rb.gravityScale;
    }

    void Update()
    {
        RevisarSuelo();
        EncontrarJugadorMasCercano();

        if (jugadorObjetivo != null)
        {
            jugadorEnRango = true;
            ActualizarMirada();

            // ¿El player está en la zona de huida?
            float distanciaAlJugador = Vector2.Distance(transform.position, jugadorObjetivo.position);

            if (distanciaAlJugador <= radioHuida)
            {
                HuirDelJugador(); // Moverse lejos
            }
            else
            {
                // Fuera de zona de huida: quedarse quieto y atacar
                if (rb != null)
                    rb.velocity = new Vector2(0, rb.velocity.y);
            }

            // Siempre lanza navajas si el player está en el área grande
            cronometroAtaque += Time.deltaTime;
            if (cronometroAtaque >= tiempoEntreNavajas)
            {
                LanzarNavaja();
                cronometroAtaque = 0;
            }
        }
        else
        {
            RegresarAFrente();
        }
    }

    void HuirDelJugador()
    {
        // Dirección opuesta al jugador (solo en X, para no volar)
        float dirX = transform.position.x - jugadorObjetivo.position.x;
        dirX = dirX > 0 ? 1f : -1f; // Normalizar a 1 o -1

        // Verificar que no huya demasiado lejos de su posición inicial
        float distanciaDeOrigen = transform.position.x - posicionInicial.x;
        bool huyendoDerecha = dirX > 0;
        bool huyendoIzquierda = dirX < 0;

        bool demasiadoLejosDerecha = distanciaDeOrigen > distanciaMaxHuida && huyendoDerecha;
        bool demasiadoLejosIzquierda = distanciaDeOrigen < -distanciaMaxHuida && huyendoIzquierda;

        if (!demasiadoLejosDerecha && !demasiadoLejosIzquierda)
        {
            rb.velocity = new Vector2(dirX * velocidadHuida, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y); // Se queda quieto en el límite
        }
    }

    void EncontrarJugadorMasCercano()
    {
        Vector2 centroReal = (Vector2)transform.position + offsetDeteccion;
        Collider2D[] jugadoresEncontrados = Physics2D.OverlapBoxAll(centroReal, tamanoDeteccion, 0f, capaJugadores);

        float distanciaCercana = Mathf.Infinity;
        Transform objetivoTemporal = null;

        foreach (Collider2D col in jugadoresEncontrados)
        {
            if (col.CompareTag("Player") || col.CompareTag("Player2"))
            {
                if (col.transform.position.y > transform.position.y - 0.5f)
                {
                    float distancia = Vector2.Distance(transform.position, col.transform.position);
                    if (distancia < distanciaCercana)
                    {
                        distanciaCercana = distancia;
                        objetivoTemporal = col.transform;
                    }
                }
            }
        }
        jugadorObjetivo = objetivoTemporal;
    }

    void RevisarSuelo()
    {
        if (detectorSuelo == null || rb == null) return;

        bool tocandoSuelo = Physics2D.OverlapCircle(detectorSuelo.position, radioDeteccionSuelo, capaSuelo);

        if (tocandoSuelo)
        {
            if (rb.velocity.y < 0)
                rb.velocity = new Vector2(rb.velocity.x, 0);
        }
    }

    void ActualizarMirada()
    {
        animator.SetBool("PlayerEnRango", true);

        float direccionX = jugadorObjetivo.position.x - transform.position.x;
        float valorAnim = direccionX > 0 ? 1f : -1f;

        animator.SetFloat("DirectionX", valorAnim);
    }

    void RegresarAFrente()
    {
        if (jugadorEnRango)
        {
            jugadorEnRango = false;
            cronometroAtaque = 0;
            animator.SetBool("PlayerEnRango", false);
            animator.SetFloat("DirectionX", 0f);

            if (rb != null)
                rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    void LanzarNavaja()
    {
        if (jugadorObjetivo == null) return;

        GameObject nuevaNavaja = Instantiate(navajaPrefab, puntoDisparo.position, Quaternion.identity);
        Vector2 direccionAtaque = jugadorObjetivo.position - puntoDisparo.position;
        nuevaNavaja.GetComponent<NavajaVuelo>().Configurar(direccionAtaque, puntoDisparo.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Player2"))
        {
            VidaJugador vidaJ1 = collision.GetComponent<VidaJugador>();
            VidaJugador2 vidaJ2 = collision.GetComponent<VidaJugador2>();

            Vector2 direccionGolpe = collision.transform.position - transform.position;

            if (vidaJ1 != null)
            {
                vidaJ1.RecibirDaño(direccionGolpe);
                ActivarEfectoHumo();
            }
            else if (vidaJ2 != null)
            {
                vidaJ2.RecibirDaño(direccionGolpe);
                ActivarEfectoHumo();
            }
        }
    }

    void ActivarEfectoHumo()
    {
        if (efectoHumo != null)
        {
            CancelInvoke("DesactivarHumo");
            efectoHumo.SetActive(true);

            Animator humoAnim = efectoHumo.GetComponent<Animator>();
            if (humoAnim != null)
                humoAnim.SetTrigger("Humo_Ataque");

            Invoke("DesactivarHumo", 0.5f);
        }
    }

    void DesactivarHumo()
    {
        if (efectoHumo != null) efectoHumo.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        // Zona de ataque (rectángulo grande) - Amarillo
        Gizmos.color = Color.yellow;
        Vector2 centroReal = (Vector2)transform.position + offsetDeteccion;
        Gizmos.DrawWireCube(centroReal, tamanoDeteccion);

        // Zona de huida (círculo pequeño) - Rojo
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioHuida);

        // Límite máximo de huida - Azul
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, distanciaMaxHuida);

        if (detectorSuelo != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(detectorSuelo.position, radioDeteccionSuelo);
        }
    }
}