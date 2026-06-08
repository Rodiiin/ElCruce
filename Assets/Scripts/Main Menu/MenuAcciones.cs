using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAcciones : MonoBehaviour
{
    public void SalirDelJuego()
    {
        Application.Quit();

        // Si estás dentro de Unity, esto detiene el modo Play automáticamente
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}