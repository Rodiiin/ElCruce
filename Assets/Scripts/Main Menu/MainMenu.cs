using UnityEngine;
using UnityEngine.SceneManagement; 
public class MainMenu : MonoBehaviour
{

    public void Play()
    {
        SceneManager.LoadScene("Test_hombre");
    }

        public void Quit()
    {
        Application.Quit();
    }
}