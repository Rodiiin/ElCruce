using System.Collections;
using UnityEngine;

public abstract class SceneTransition : MonoBehaviour
{
    // Obliga a las transiciones a definir cómo aparecen (ej. de negro a transparente)
    public abstract IEnumerator AnimateTransitionIn();

    // Obliga a las transiciones a definir cómo desaparecen (ej. de transparente a negro)
    public abstract IEnumerator AnimateTransitionOut();
}