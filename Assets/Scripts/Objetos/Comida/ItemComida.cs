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
        if (collision.CompareTag("Player") || collision.CompareTag("Player2"))
        {
            // Buscamos el componente genérico de vida
            VidaJugador scriptVida = collision.GetComponent<VidaJugador>();
            VidaJugador2 scriptVida2 = collision.GetComponent<VidaJugador2>();

            if (scriptVida != null)
            {
                scriptVida.Curar(puntosDeVida);
                scriptVida2.Curar(puntosDeVida);
                Destroy(gameObject);
            }
        }
    }    
}
