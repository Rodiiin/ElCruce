using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PruebaGolpe : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si chocamos con el jugador
        if (collision.gameObject.CompareTag("Player"))
        {
            // Calculamos la dirección del golpe (desde el enemigo hacia el jugador)
            Vector2 direccion = collision.transform.position - transform.position;
            
            // Llamamos al método del jugador
            collision.gameObject.GetComponent<MovimientoUnico>().RecibirDaño(direccion);
        }
    }
}
