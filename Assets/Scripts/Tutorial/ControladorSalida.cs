using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorSalida : MonoBehaviour
{
    public GameObject panelConfirmacion; // Arrastra el Panel_ConfirmacionSalida aquí
    public GameObject iconoE;            // Un texto o imagen que diga "Presiona E"
    public string nombreSiguienteNivel = "Nivel_1";
    
    private bool estaCerca = false;

    void Update()
    {
        // Si estoy cerca de la puerta y presiono E, muestro el menú
        if (estaCerca && Input.GetKeyDown(KeyCode.E))
        {
            AbrirMenu();
        }
    }

    void AbrirMenu()
    {
        panelConfirmacion.SetActive(true);
        iconoE.SetActive(false); // Ocultamos la E mientras el menú está abierto
        Time.timeScale = 0f;    // Pausamos para que piensen con calma
    }

    // --- FUNCIONES PARA LOS BOTONES ---

    public void BotonAceptar()
    {
        Time.timeScale = 1f; // ¡Muy importante reanudar el tiempo!
        SceneManager.LoadScene(nombreSiguienteNivel);
    }

    public void BotonCancelar()
    {
        panelConfirmacion.SetActive(false);
        Time.timeScale = 1f; // Reanudamos el juego
        if (estaCerca) iconoE.SetActive(true); // Volvemos a mostrar la E
    }

    // --- DETECTAR SI EL JUGADOR ESTÁ EN LA PUERTA ---

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Player2"))
        {
            estaCerca = true;
            iconoE.SetActive(true); // Aparece el aviso de "Presiona E"
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Player2"))
        {
            estaCerca = false;
            iconoE.SetActive(false);
            panelConfirmacion.SetActive(false); // Por si se alejan con el menú abierto
        }
    }
}
