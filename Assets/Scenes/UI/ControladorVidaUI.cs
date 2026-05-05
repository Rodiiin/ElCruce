using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorVidaUI : MonoBehaviour
{
    [Header("Configuración Niño (P1)")]
    public List<GameObject> corazonesNiño;

    [Header("Configuración Niña (P2)")]
    public List<GameObject> corazonesNiña;

    // Esta es la función mágica que apaga los corazones
    public void ActualizarCorazones(int vidaActual, bool esNiño)
    {
        // Elegimos qué lista de corazones usar
        List<GameObject> listaAUsar = esNiño ? corazonesNiño : corazonesNiña;

        for (int i = 0; i < listaAUsar.Count; i++)
        {
            // Si el número de vida es mayor al índice, el corazón sigue activo
            // Ejemplo: Si vida es 2, el corazón 0 y 1 se quedan, el 2 se apaga
            listaAUsar[i].SetActive(i < vidaActual);
        }
    }
}
