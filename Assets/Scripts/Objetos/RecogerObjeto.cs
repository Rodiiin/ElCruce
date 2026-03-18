using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecogerObjeto : MonoBehaviour
{
    [Header("Puntos de Sujeción")]
    public Transform puntoSujecion;  // Mano 1 (Roca)
    public Transform puntoSujecion2; // Mano 2 (Llave)
    
    
    [Header("Configuración Lanzamiento")]
    public float fuerzaLanzamiento = 10f;
    public Vector2 anguloLanzamiento = new Vector2(1, 1); // Ángulo fijo (Diagonal)
    public float tiempoRespawn = 3f;

    // Referencias para la Roca 
    private GameObject objetoActual = null;

    // Referencias para las Llaves (lo que se guarda)
    private GameObject llaveMano2 = null;


    //Cercania del objeto
    private bool cercaDeObjeto = false;
    private GameObject objetoEnRango = null;
    private SpriteRenderer sr;
    

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }


    void Update()
    {
        // --- RECOGER (Presionando la tecla 1) ---
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (cercaDeObjeto && objetoEnRango != null)
            {
                // Si es una llave, intentamos recogerla
                if (objetoEnRango.CompareTag("Llave"))
                {
                    RecogerLlave(objetoEnRango);
                }
                // Si es una roca, solo si no tenemos ya una
                else if (objetoEnRango.CompareTag("Roca"))
                {
                    RecogerRoca(objetoEnRango);
                }
            }
        }
        
        // --- TIRAR ROCA (Tecla E) ---
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (objetoActual != null)
            {
                LanzarRoca();
            }
        }

    }



    void RecogerRoca(GameObject roca)
    {

        // Solo recogemos si la MANO 1 (puntoSujecion) está libre
        if (objetoActual == null)
        {
            objetoActual = roca;
            objetoActual.transform.position = puntoSujecion.position;
            objetoActual.transform.SetParent(puntoSujecion);
            
            Rigidbody2D rb = objetoActual.GetComponent<Rigidbody2D>();
            if (rb != null) rb.simulated = false;

            Debug.Log("Roca recogida en Mano 1");
        }
        else
        {
            Debug.Log("Mano 1 ocupada, ya cargas una roca.");
        }
    }

    void RecogerLlave(GameObject llave)
    {
        // Solo recogemos si la MANO 2 (puntoSujecion2) está libre
        if (llaveMano2 == null)
        {
            llaveMano2 = llave;
            llave.transform.position = puntoSujecion2.position;
            llave.transform.SetParent(puntoSujecion2);
            
            Rigidbody2D rb = llave.GetComponent<Rigidbody2D>();
            if (rb != null) rb.simulated = false;

            Animator anim = llave.GetComponent<Animator>();
            if (anim != null) anim.enabled = false;

            cercaDeObjeto = false; 
            Debug.Log("Llave recogida en Mano 2");
        }
        else
        {
            Debug.Log("Mano 2 ocupada, ya cargas una llave.");
        }
    }

    void LanzarRoca()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.SetTrigger("Throw");

        GameObject rocaSoltada = objetoActual;
        rocaSoltada.transform.SetParent(null);
        
        Rigidbody2D rb = rocaSoltada.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = true;
            rb.bodyType = RigidbodyType2D.Dynamic;

            float direccionX = (sr != null && sr.flipX) ? 1f : -1f;
            Vector2 fuerzaFinal = new Vector2(direccionX * anguloLanzamiento.x, anguloLanzamiento.y).normalized * fuerzaLanzamiento;
            rb.AddForce(fuerzaFinal, ForceMode2D.Impulse);
        }

        RocaLogica logica = rocaSoltada.GetComponent<RocaLogica>();
        if (logica != null) logica.IniciarRespawn(tiempoRespawn);

        objetoActual = null;
    }


    // Detectar si estamos cerca de una roca
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detectamos si es Roca O Llave
        if (collision.CompareTag("Roca") || collision.CompareTag("Llave"))
        {
            cercaDeObjeto = true;
            objetoEnRango = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Roca") || collision.CompareTag("Llave"))
        {
            cercaDeObjeto = false;
            objetoEnRango = null;
        }
    }

}
