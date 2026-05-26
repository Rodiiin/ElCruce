using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coleccionable_Canicas : MonoBehaviour
{
    [Header("Configuracion UI")]
    public GameObject panelLogro; // Arrastra aqui el Panel_Logro

    private bool recolectado = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificamos si quien lo toca es el jugador
        if (collision.CompareTag("Player") && !recolectado)
        {
            recolectado = true;
            MostrarLogro();
        }
        if (collision.CompareTag("Player2") && !recolectado)
        {
            recolectado = true;
            MostrarLogro();
        }
    }

    void MostrarLogro()
    {
        // Mostramos el panel y pausamos el tiempo
        panelLogro.SetActive(true);
        Time.timeScale = 0f; 
    }

    void Update()
    {
        // Si el panel esta activo y presionan Espacio, quitamos el panel y seguimos
        if (recolectado && Input.GetKeyDown(KeyCode.Space))
        {
            Continuar();
        }
    }

    void Continuar()
    {
        Time.timeScale = 1f; // Reanudamos el tiempo
        panelLogro.SetActive(false);
        Destroy(gameObject); // El rosario desaparece de la escena
    }
}
