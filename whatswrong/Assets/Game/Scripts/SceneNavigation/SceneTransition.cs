using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scripts
{
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField] private CanvasGroup transitionOverlay;
        [SerializeField] private float fadeOutDuration = 0.2f;
        [SerializeField] private float fadeInDuration = 0.35f;

        public IEnumerator FadeIn(string sceneName)
        {
            if (transitionOverlay != null)
                transitionOverlay.alpha = 1f;

            yield return SceneManager.LoadSceneAsync(
                sceneName,
                LoadSceneMode.Additive
            );
            
            if (transitionOverlay != null)
                yield return FadeOverlay(0f, fadeInDuration);
        }

        public IEnumerator FadeOut(string sceneName)
        {
            if (transitionOverlay != null)
                yield return FadeOverlay(1f, fadeOutDuration);

            yield return SceneManager.UnloadSceneAsync(sceneName);
            
        }
        
        private IEnumerator FadeOverlay(float targetAlpha, float duration)
        {
            if (transitionOverlay == null)
                yield break;

            float startAlpha = transitionOverlay.alpha;
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp01(time / duration);
                transitionOverlay.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                yield return null;
            }

            transitionOverlay.alpha = targetAlpha;
        }

        
    }

}