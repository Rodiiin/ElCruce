using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavajaVuelo : MonoBehaviour
{
    public float velocidad = 15f;
    public float tiempoVida = 3f;
    private Vector2 direccionVuelo;
    private Vector2 origenAtaque;

    public void Configurar(Vector2 dir, Vector2 origen)
    {
        direccionVuelo = dir.normalized;
        origenAtaque = origen;
        // Rotar la navaja para que apunte hacia donde vuela
        float anguloBase = Mathf.Atan2(direccionVuelo.y, direccionVuelo.x) * Mathf.Rad2Deg;

        

        transform.rotation = Quaternion.Euler(0, 0, anguloBase );
        
        Destroy(gameObject, tiempoVida); // Se destruye sola tras X segundos
    }

    void Update()
    {
        transform.Translate(Vector2.right * velocidad * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Player2"))
        {
            // 1. Buscamos el script de vida en el jugador que tocamos
            VidaJugador vida = collision.GetComponent<VidaJugador>();

            if (vida != null)
            {
                // 2. Calculamos la dirección del golpe (desde la navaja hacia el jugador)
                // Usamos transform.right porque la navaja vuela hacia su "derecha" local
                Vector2 direccionGolpe = (Vector2)collision.transform.position - origenAtaque;
                
                // 3. Aplicamos el daño
                vida.RecibirDaño(direccionGolpe);
            }

            Destroy(gameObject); // Desaparece al impactar
        }
    }
}
