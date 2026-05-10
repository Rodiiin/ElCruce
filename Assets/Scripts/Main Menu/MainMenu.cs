using UnityEngine;
using UnityEngine.SceneManagement; 
public class MainMenu : MonoBehaviour
{

    public void Play()
    {
        SceneManager.LoadScene("Nivel1");
    }

        public void Quit()
    {
        Application.Quit();
    }
}