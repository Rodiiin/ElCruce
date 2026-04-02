using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampaViento : MonoBehaviour
{
    [Header("Configuración de Fuerza")]
    public float fuerzaEmpuje = 10f;
    
    [Header("Visualización")]
    public Color colorGizmo = new Color(0, 1, 1, 0.3f);

    private BoxCollider2D areaEfecto;

    void Awake()
    {
        areaEfecto = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Detectamos si es uno de los jugadores
        if (collision.CompareTag("Player") || collision.CompareTag("Player2"))
        {
            Rigidbody2D rbJugador = collision.GetComponent<Rigidbody2D>();
            
            if (rbJugador != null)
            {
                // DETECCIÓN DE DIRECCIÓN:
                // Si el transform.localScale.x es positivo, mira a la derecha (1)
                // Si es negativo (porque hiciste flip), mira a la izquierda (-1)
                float direccionX = Mathf.Sign(transform.localScale.x);
                
                Vector2 fuerza = new Vector2(direccionX * fuerzaEmpuje, 0);
                
                // Aplicamos la fuerza de forma constante mientras esté dentro
                rbJugador.AddForce(fuerza, ForceMode2D.Force);
            }
        }
    }

    // Esto permite ver el área de efecto en la ventana de Scene sin darle a Play
    private void OnDrawGizmos()
    {
        if (areaEfecto == null) areaEfecto = GetComponent<BoxCollider2D>();

        if (areaEfecto != null)
        {
            Gizmos.color = colorGizmo;
            // Dibujamos un cubo sólido que representa el área exacta del Collider
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(areaEfecto.offset, areaEfecto.size);
        }
    }
}
