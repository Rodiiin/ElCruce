using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControladorVidaUI : MonoBehaviour
{
    [Header("Configuración Niño (P1)")]
    public List<GameObject> corazonesNiño;
    public Image iconoNiño;
    public Sprite cabezaNiñoNormal;   // Sprite original (agrega este campo)
    public Sprite cabezaNiñoMuerto;

    [Header("Configuración Niña (P2)")]
    public List<GameObject> corazonesNiña;
    public Image iconoNiña;
    public Sprite cabezaNiñaNormal;   // Sprite original (agrega este campo)
    public Sprite cabezaNiñaMuerto;

    public void ActualizarCorazones(int vidaActual, bool esNiño)
    {
        List<GameObject> listaAUsar = esNiño ? corazonesNiño : corazonesNiña;

        for (int i = 0; i < listaAUsar.Count; i++)
        {
            listaAUsar[i].SetActive(i < vidaActual);
        }

        if (vidaActual <= 0)
            CambiarAEstadoMuerto(esNiño);
    }

    public void RestaurarIcono(bool esNiño)
    {
        if (esNiño)
        {
            iconoNiño.sprite = cabezaNiñoNormal;
            iconoNiño.color = Color.white;
        }
        else
        {
            iconoNiña.sprite = cabezaNiñaNormal;
            iconoNiña.color = Color.white;
        }
    }

    private void CambiarAEstadoMuerto(bool esNiño)
    {
        if (esNiño)
        {
            iconoNiño.sprite = cabezaNiñoMuerto;
            iconoNiño.color = Color.gray;
        }
        else
        {
            iconoNiña.sprite = cabezaNiñaMuerto;
            iconoNiña.color = Color.gray;
        }
    }
}