using System.Collections;
using UnityEngine;

namespace ElCruce.UI.Menus
{
    public abstract class SceneTransition : MonoBehaviour
    {
        public abstract IEnumerator AnimateTransitionIn();
        public abstract IEnumerator AnimateTransitionOut();
    }
}