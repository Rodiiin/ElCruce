using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformaQueCae : MonoBehaviour
{
    [Header("Configuración")]
    public float tiempoEspera = 0.8f; // Cuánto tarda en caer
    public float velocidadTemblor = 0.1f; // Qué tan fuerte tiembla
    public float tiempoDestruccion = 2f; // Cuánto tarda en desaparecer tras caer
    public float tiempoRespawn = 2.0f; // Tiempo para que reaparezca

    private Rigidbody2D rb;
    private Vector3 posicionOriginal;
    private bool estaCayendo = false;

    // Referencias para ocultar la plataforma
    private SpriteRenderer[] renderers;
    private Collider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        // Buscamos los sprites en el padre y en los 3 bloques hijos
        renderers = GetComponentsInChildren<SpriteRenderer>();
        posicionOriginal = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Detectamos si el que pisó la plataforma es un jugador
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Player2"))
        {
            // Solo activamos la caída si no está cayendo ya
            if (!estaCayendo)
            {
                StartCoroutine(SecuenciaCaida());
            }
        }
    }

    private IEnumerator SecuenciaCaida()
    {
        estaCayendo = true;

        // --- FASE 1: Temblor ---
        float tiempoPasado = 0;
        while (tiempoPasado < tiempoEspera)
        {
            transform.position = posicionOriginal + (Vector3)Random.insideUnitCircle * velocidadTemblor;
            tiempoPasado += Time.deltaTime;
            yield return null;
        }

        // --- FASE 2: Caída ---
        rb.bodyType = RigidbodyType2D.Dynamic;

        //Congelamos el movimiento en X y la rotación para que caiga recto hacia abajo sin moverse horizontalmente ni girar
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        // Esperamos un poco antes de "hacerla desaparecer"
        yield return new WaitForSeconds(1.5f); 

        // --- FASE 3: Desaparecer (Invisible y sin choque) ---
        col.enabled = false;
        foreach (SpriteRenderer s in renderers) s.enabled = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector2.zero;

        //Al volver a Kinematic, reseteamos las constraints para que no interfieran con el Respawn
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.velocity = Vector2.zero;

        // --- FASE 4: Respawn ---
        yield return new WaitForSeconds(tiempoRespawn);
        
        // Resetear todo a su estado inicial
        transform.position = posicionOriginal;
        rb.constraints = RigidbodyConstraints2D.None;
        col.enabled = true;
        foreach (SpriteRenderer s in renderers) s.enabled = true;
        estaCayendo = false;
    }

}
