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
            // 1. Intentamos obtener ambos scripts
            VidaJugador vidaJ1 = collision.GetComponent<VidaJugador>();
            VidaJugador2 vidaJ2 = collision.GetComponent<VidaJugador2>();

            // 2. Calculamos la dirección del golpe (desde el origen del ataque hacia el jugador)
            Vector2 direccionGolpe = (Vector2)collision.transform.position - origenAtaque;
            
            // 3. Aplicamos el daño según quién sea el impactado
            if (vidaJ1 != null)
            {
                vidaJ1.RecibirDaño(direccionGolpe);
                Destroy(gameObject); // Desaparece al impactar al J1
            }
            else if (vidaJ2 != null)
            {
                vidaJ2.RecibirDaño(direccionGolpe);
                Destroy(gameObject); // Desaparece al impactar al J2
            }
        }
        
        // Opcional: Si quieres que la navaja se destruya al chocar con paredes (Suelo)
        if (collision.gameObject.layer == LayerMask.NameToLayer("Suelo")) 
        {
            Destroy(gameObject);
        }
    }
}
