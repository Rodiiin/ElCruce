using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections;

public class ControladorPausa : MonoBehaviour
{
    [Header("Referencias de UI")]
    [SerializeField] private GameObject Panel_Pausa; 
    [SerializeField] private GameObject LogoPausa; 
    
    [Header("Animación del Sobre")]
    [SerializeField] private Animator AbrirMenu; 

    private bool juegoPausado = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (juegoPausado)   
                Continuar();
            else                
                Pausar();
        }
    }

    public void Pausar()
    {
        juegoPausado = true;
        Panel_Pausa.SetActive(true); 
        LogoPausa.SetActive(false); 
        
        if (AbrirMenu != null)
        {
            AbrirMenu.speed = 1f; 
            AbrirMenu.Play("AbrirMenu", 0, 0f); 
        }

        Time.timeScale = 0f; 
    }

    public void Continuar()
    {
        if (AbrirMenu != null)
        {
            AbrirMenu.speed = -1f; // 3. Ponemos la velocidad en negativo para que la carta se guarde sola
        }

        StartCoroutine(EsperarYDesactivar());
    }

    private IEnumerator EsperarYDesactivar()
    {
        juegoPausado = false;
        Time.timeScale = 1f; 
        LogoPausa.SetActive(true); 

        
        yield return new WaitForSecondsRealtime(0.4f); 

        Panel_Pausa.SetActive(false); 
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SalirAlMenuPrincipal(string MainMenu)
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(MainMenu);
    }
}