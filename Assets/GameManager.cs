using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ElCruce.UI.Menus
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private SceneTransition currentTransition; 

        private void Start()
        {
            if (currentTransition != null)
            {
                StartCoroutine(currentTransition.AnimateTransitionIn());
            }
        }

        public void SwitchScene(string sceneName)
        {
            StartCoroutine(LoadSceneSequence(sceneName));
        }

        private IEnumerator LoadSceneSequence(string sceneName)
        {
            if (currentTransition != null)
            {
                yield return StartCoroutine(currentTransition.AnimateTransitionOut());
            }

            SceneManager.LoadScene(sceneName);
        }
    }
}