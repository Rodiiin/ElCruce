using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PruebaGolpe : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verificamos si chocamos con cualquier objeto que tenga el Tag de jugador
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Player2"))
        {
            // Buscamos el nuevo componente de vida (que ahora ambos comparten)
            VidaJugador scriptVida = collision.gameObject.GetComponent<VidaJugador>();

            if (scriptVida != null)
            {
                // Calculamos la dirección del golpe (desde el enemigo hacia el jugador)
                Vector2 direccion = collision.transform.position - transform.position;
                
                // Llamamos al método RecibirDaño en el script de Vida
                scriptVida.RecibirDaño(direccion);
            }
        }
    }
}
