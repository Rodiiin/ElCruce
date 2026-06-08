using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    [Header("Referencias Jugadores")]
    public VidaJugador vidaJugador1;
    public VidaJugador2 vidaJugador2;

    [Header("UI Game Over")]
    public GameObject panelGameOver;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip musicaGameOver;

    private bool gameOverActivado = false;

    void Start()
    {
        if (panelGameOver != null) panelGameOver.SetActive(false);
    }

    void Update()
    {
        if (gameOverActivado) return;

        bool j1Muerto = vidaJugador1 != null && vidaJugador1.estaMuerto;
        bool j2Muerto = vidaJugador2 != null && vidaJugador2.estaMuerto;

        if (j1Muerto && j2Muerto)
            StartCoroutine(ActivarGameOver());
    }

    IEnumerator ActivarGameOver()
    {
        gameOverActivado = true;
        yield return new WaitForSeconds(1f);
        Time.timeScale = 0f;
    
        // Detener música actual y poner la de Game Over
        AudioListener.pause = true; // pausa todos los audios del juego
        if (audioSource != null && musicaGameOver != null)
        {
            audioSource.ignoreListenerPause = true; // este sí se escucha aunque el juego esté pausado
        audioSource.clip = musicaGameOver;
        audioSource.Play();
        }
    
        if (panelGameOver != null) panelGameOver.SetActive(true);
    }

    public void Reintentar()
    {
        AudioListener.pause = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MenuPrincipal()
    {
        AudioListener.pause = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // cambia el 0 por el índice de tu menú principal
    }
    
}