using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deckard : MonoBehaviour
{
    [Header("Detección")]
    public float radioDeteccion = 5f;
    public Vector2 tamanoDeteccion = new Vector2(10f, 5f); // Ancho y Alto
    public Vector2 offsetDeteccion = new Vector2(0f, 2.5f);
    public LayerMask capaJugadores; // Selecciona "Player" en el Inspector
    
   
    [Header("Floor detection")]
    public Transform detectorSuelo; 
    private float gravedadOriginal;
    private Rigidbody2D rb;
    
    
    private Transform jugadorObjetivo;
    private Animator animator;
    private bool jugadorEnRango = false;

    [Header("Ataque")]
    public GameObject navajaPrefab;
    public Transform puntoDisparo;
    public float tiempoEntreNavajas = 2f;
    private float cronometroAtaque;

    public GameObject efectoHumo; 

    void Start()
    {
        animator = GetComponent<Animator>();
        gravedadOriginal = rb.gravityScale; 

    }

    void Update()
    {
        EncontrarJugadorMasCercano();

        // Si encontramos a alguien dentro del RECTÁNGULO
        if (jugadorObjetivo != null)
        {
            jugadorEnRango = true;
            ActualizarMirada();

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

    void EncontrarJugadorMasCercano()
    {
        Vector2 centroReal = (Vector2)transform.position + offsetDeteccion;
        Collider2D[] jugadoresEncontrados = Physics2D.OverlapBoxAll(centroReal, tamanoDeteccion, 0f, capaJugadores);        
        
        float distanciaCercana = Mathf.Infinity;
        Transform objetivoTemporal = null;

        foreach (Collider2D col in jugadoresEncontrados)
        {
            // Verificamos que sea P1 o P2 por su Tag
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
    void ActualizarMirada()
    {
        animator.SetBool("PlayerEnRango", true);

        // Calculamos la dirección X (Posición Jugador - Posición Deckard)
        float direccionX = jugadorObjetivo.position.x - transform.position.x;

        // Normalizamos el valor para que sea 1 (derecha) o -1 (izquierda)
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
        }
    }

    void LanzarNavaja()
    {
        if (jugadorObjetivo == null) return;

        // Instanciar la navaja
        GameObject nuevaNavaja = Instantiate(navajaPrefab, puntoDisparo.position, Quaternion.identity);
        
        // Calcular dirección hacia la ubicación actual del player
        Vector2 direccionAtaque = jugadorObjetivo.position - puntoDisparo.position;
        
        // Configurar el vuelo
        nuevaNavaja.GetComponent<NavajaVuelo>().Configurar(direccionAtaque, puntoDisparo.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si lo que toca a Deckard es un Jugador...
        if (collision.CompareTag("Player") || collision.CompareTag("Player2"))
        {
            // Intentamos obtener cualquiera de los dos scripts de vida
            VidaJugador vidaJ1 = collision.GetComponent<VidaJugador>();
            VidaJugador2 vidaJ2 = collision.GetComponent<VidaJugador2>();

            
            // Calculamos dirección del golpe (Desde Deckard hacia el Player)
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
        // Si reciclamos el objeto efectoHumo que usamos con Roy:
        if (efectoHumo != null)
        {
            CancelInvoke("DesactivarHumo");

            efectoHumo.SetActive(true);

            Animator humoAnim = efectoHumo.GetComponent<Animator>();

            if (humoAnim != null)
            {
                humoAnim.SetTrigger("Humo_Ataque");
            }
            // Lo apagamos después de un momento
            Invoke("DesactivarHumo", 0.5f); 
        }
    }

    void DesactivarHumo()
    {
        if(efectoHumo != null) efectoHumo.SetActive(false);
    }


    // Dibujamos el círculo de rango en la pestaña Scene para poder ajustarlo fácil
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 centroReal = (Vector2)transform.position + offsetDeteccion;
        Gizmos.DrawWireCube(centroReal, tamanoDeteccion);

        if (detectorSuelo != null)
        {
            // Dibujamos una esfera roja en la posición del detector para ver el rango
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(detectorSuelo.position, radioDeteccion);
        }
    }
}
