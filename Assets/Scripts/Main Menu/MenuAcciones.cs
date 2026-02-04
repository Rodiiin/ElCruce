using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MenuAcciones : MonoBehaviour
{
    public void SalirDelJuego()
    {
        Application.Quit();
        // #if UNITY_EDITOR
        //     EditorApplication.isPlaying = false;
        // #endif
    }
}
