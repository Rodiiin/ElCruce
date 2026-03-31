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
        if (collision.CompareTag("Player"))
        {
            // Buscamos el componente genérico de vida
            VidaJugador scriptVida = collision.GetComponent<VidaJugador>();

            if (scriptVida != null)
            {
                scriptVida.Curar(puntosDeVida);
                Destroy(gameObject);
            }
        }
    }    
}
