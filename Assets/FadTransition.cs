using System.Collections;
using UnityEngine;

namespace ElCruce.UI.Menus
{
    public class FadTransition : SceneTransition
    {
        [Header("Referencias UI")]
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Configuración del Fundido")]
        [SerializeField] private float duration = 1f;
        
        [Tooltip("Modifica la curva en el Inspector para cambiar el ritmo del fundido (Suave al inicio/final).")]
        [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        // Con 'override' le avisamos a Unity que estamos cumpliendo con el contrato de entrada (Aclarar pantalla)
        public override IEnumerator AnimateTransitionIn()
        {
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime; 
                float normalizedTime = timer / duration;

                // Evaluamos el tiempo a través de la curva suave
                float curveValue = transitionCurve.Evaluate(normalizedTime);

                // Va de Negro (1) a Transparente (0) usando la curva
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, curveValue); 
                yield return null;
            }
            canvasGroup.alpha = 0f;
        }

        // Con 'override' le avisamos a Unity que estamos cumpliendo con el contrato de salida (Oscurecer pantalla)
        public override IEnumerator AnimateTransitionOut()
        {
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;
                float normalizedTime = timer / duration;

                // Evaluamos el tiempo a través de la curva suave
                float curveValue = transitionCurve.Evaluate(normalizedTime);

                // Va de Transparente (0) a Negro (1) usando la curva
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, curveValue); 
                yield return null;
            }
            canvasGroup.alpha = 1f;
        }
    }
}