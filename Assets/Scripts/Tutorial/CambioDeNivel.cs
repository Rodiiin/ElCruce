using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioDeNivel : MonoBehaviour
{
    public string nombreNivel; // Escribe "Nivel_1" en el Inspector

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el Niño o la Niña tocan la puerta
        if (collision.CompareTag("Player") || collision.CompareTag("Player2"))
        {
            SceneManager.LoadScene(nombreNivel);
        }
    }
}
