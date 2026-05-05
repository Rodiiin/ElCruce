using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControladorVidaUI : MonoBehaviour
{
    [Header("Configuración Niño (P1)")]
    public List<GameObject> corazonesNiño;
    public Image iconoNiño; // Arrastra el objeto de la cabecita aquí
    public Sprite cabezaNiñoMuerto; // Arrastra el sprite de ojos cerrados aquí

    [Header("Configuración Niña (P2)")]
    public List<GameObject> corazonesNiña;
    public Image iconoNiña;
    public Sprite cabezaNiñaMuerto;

    public void ActualizarCorazones(int vidaActual, bool esNiño)
    {
        List<GameObject> listaAUsar = esNiño ? corazonesNiño : corazonesNiña;

        for (int i = 0; i < listaAUsar.Count; i++)
        {
            listaAUsar[i].SetActive(i < vidaActual);
        }

        // Si la vida llega a 0, cambiamos la cara
        if (vidaActual <= 0)
        {
            CambiarAEstadoMuerto(esNiño);
        }
    }

    private void CambiarAEstadoMuerto(bool esNiño)
    {
        if (esNiño)
        {
            iconoNiño.sprite = cabezaNiñoMuerto;
            iconoNiño.color = Color.gray; // Esto lo vuelve gris automáticamente
        }
        else
        {
            iconoNiña.sprite = cabezaNiñaMuerto;
            iconoNiña.color = Color.gray;
        }
    }
}
