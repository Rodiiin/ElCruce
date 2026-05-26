using System.Collections;
using UnityEngine;

namespace ElCruce.UI.Menus
{
    public class FadTransition : SceneTransition
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float duration = 1f;

        // Con 'override' le avisamos a Unity que estamos cumpliendo con el contrato de entrada
        public override IEnumerator AnimateTransitionIn()
        {
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime; 
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / duration); 
                yield return null;
            }
            canvasGroup.alpha = 0f;
        }

        // Con 'override' le avisamos a Unity que estamos cumpliendo con el contrato de salida
        public override IEnumerator AnimateTransitionOut()
        {
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / duration); 
                yield return null;
            }
            canvasGroup.alpha = 1f;
        }
    }
}