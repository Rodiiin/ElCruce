using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecogerObjeto : MonoBehaviour
{
    [Header("Configuración")]
    public Transform puntoSujecion; // Un objeto vacío hijo del jugador donde irá la roca
    public float fuerzaLanzamiento = 10f;
    public Vector2 anguloLanzamiento = new Vector2(1, 1); // Ángulo fijo (Diagonal)

    private GameObject rocaActual = null;
    private bool cercaDeRoca = false;
    private GameObject rocaEnRango = null;
    private SpriteRenderer sr;
    public float tiempoRespawn = 3f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }


    void Update()
    {
        // 1. Recoger con la tecla 1 (Alpha1 es el 1 del teclado arriba)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (rocaActual == null && cercaDeRoca)
            {
                Recoger();
            }
            else if (rocaActual != null)
            {
                Lanzar();
            }
        }
    }

    void Recoger()
    {
        rocaActual = rocaEnRango;
        rocaActual.transform.position = puntoSujecion.position;
        rocaActual.transform.SetParent(puntoSujecion); // Se hace hijo del jugador
        
        // Desactivamos física mientras la cargamos
        Rigidbody2D rb = rocaActual.GetComponent<Rigidbody2D>();
        rb.simulated = false; 
    }

    void Lanzar()
    {
        // 1. Intentar activar la animación de tirar, reciclandola con la animación de atacar
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Throw"); 
        }

        // Guardamos la referencia de la roca antes de soltarla
        GameObject rocaSoltada = rocaActual;

        rocaActual.transform.SetParent(null);
        
        Rigidbody2D rb = rocaActual.GetComponent<Rigidbody2D>();
        rb.simulated = true;
        rb.bodyType = RigidbodyType2D.Dynamic;

        // Si flipX es true, el personaje mira a la IZQUIERDA (en tus scripts de jugador Hombre)
        // Si flipX es false, mira a la DERECHA
        float direccionX;
        if (sr != null) {
            // Invertimos la lógica según cómo configuraste el flipX en tu script de movimiento
            direccionX = sr.flipX ? 1f : -1f; 
        } else {
            direccionX = 1f;
        }

        Vector2 fuerzaFinal = new Vector2(direccionX * anguloLanzamiento.x, anguloLanzamiento.y).normalized * fuerzaLanzamiento;
        rb.AddForce(fuerzaFinal, ForceMode2D.Impulse);

        // -- Llamamos al respawn de la roca ---
        RocaLogica logicaRoca = rocaSoltada.GetComponent<RocaLogica>();
        if (logicaRoca != null)
        {
            logicaRoca.IniciarRespawn(tiempoRespawn);
        }

        rocaActual = null;
    }

    // Detectar si estamos cerca de una roca
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Roca"))
        {
            cercaDeRoca = true;
            rocaEnRango = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Roca"))
        {
            cercaDeRoca = false;
            rocaEnRango = null;
        }
    }

}
