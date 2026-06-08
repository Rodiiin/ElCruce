using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAcciones : MonoBehaviour
{
    public void SalirDelJuego()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
