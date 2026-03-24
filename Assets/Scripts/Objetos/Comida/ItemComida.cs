using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemComida : MonoBehaviour
{
    [Header("Configuración")]
    public int puntosDeVida = 1; // Cuánta vida recupera
    public GameObject efectoParticulas;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ambos jugadores deben tener el Tag "Player" para que esto entre
        if (collision.CompareTag("Player"))
        {
            // Intentamos obtener el script del Jugador 1
            MovimientoUnico j1 = collision.GetComponent<MovimientoUnico>();
            // Intentamos obtener el script del Jugador 2
            MovimientoJugador2 j2 = collision.GetComponent<MovimientoJugador2>();

            bool fueCurado = false;

            if (j1 != null)
            {
                j1.Curar(puntosDeVida);
                fueCurado = true;
            }
            else if (j2 != null)
            {
                j2.Curar(puntosDeVida);
                fueCurado = true;
            }

            // Si alguno de los dos fue curado, procesamos la desaparición del item
            if (fueCurado)
            {
                if (efectoParticulas != null)
                {
                    Instantiate(efectoParticulas, transform.position, Quaternion.identity);
                }

                Destroy(gameObject);
            }
        }
    }    
}
