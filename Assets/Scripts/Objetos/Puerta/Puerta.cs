using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    
    private Animator anim;
    private BoxCollider2D colisionador;
    private bool estaAbierta = false;

    [Header("Configuración de Interacción")]
    public Vector2 areaSize = new Vector2(2f, 1f); // Tamaño del área
    public Vector2 areaOffset = Vector2.zero;      // Desplazamiento
    public LayerMask capaJugador;                  // Selecciona "Player"

    void Start()
    {
        // Obtenemos las referencias de la propia puerta
        anim = GetComponent<Animator>();
        colisionador = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (estaAbierta) return;

        // Creamos un cuadro invisible en el aire basado en nuestra configuración
        Vector2 posicionCentro = (Vector2)transform.position + areaOffset;
        Collider2D hit = Physics2D.OverlapBox(posicionCentro, areaSize, 0f, capaJugador);

        // Si el jugador está dentro del cuadro y presiona 'G'
        if (hit != null && Input.GetKeyDown(KeyCode.G))
        {
            RecogerObjeto inventario = hit.GetComponent<RecogerObjeto>();

            if (inventario != null && inventario.EntregarLlave())
            {
                Abrir();
            }
            else
            {
                Debug.Log("No tienes la llave o el componente RecogerObjeto.");
            }
        }
    }

    void Abrir()
    {
        estaAbierta = true;
        
        // 1. Ejecutamos la animación 
        if (anim != null) anim.SetTrigger("Abrir");

        // 2. Quitamos el BoxCollider para que el personaje pase
        if (colisionador != null) colisionador.enabled = false;

        Debug.Log("Puerta abierta");
    }

    private void OnDrawGizmos()
    {
        // Color verde si está cerrada, rojo si está abierta (opcional)
        Gizmos.color = estaAbierta ? Color.red : Color.cyan;
        
        // Dibujamos el cubo usando la posición, el offset y el tamaño
        Vector3 posicionCentro = transform.position + new Vector3(areaOffset.x, areaOffset.y, 0);
        Gizmos.DrawWireCube(posicionCentro, new Vector3(areaSize.x, areaSize.y, 1));
    }
}
