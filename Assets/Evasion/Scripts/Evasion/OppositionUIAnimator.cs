using Pixelplacement;
using UnityEngine;

namespace Evasion
{
    public class OppositionUIAnimator : MonoBehaviour
    {
        [SerializeField] private GameObject imageThinking;
        [SerializeField] private GameObject imageSearching;

        private void Start()
        {
            imageSearching.transform.localScale = 0.6f * Vector3.one;
            Tween.LocalScale(
                imageSearching.transform,
                Vector3.one,
                AnimationProperties.UITweenDuration,
                0.0f,
                Tween.EaseInOut,
                Tween.LoopType.PingPong
            );

            const float rotation = 40.0f;
            imageThinking.transform.Rotate(imageSearching.transform.forward, -rotation);
            Tween.LocalRotation(
                imageThinking.transform,
                new Vector3(0.0f, 0.0f, rotation),
                AnimationProperties.UITweenDuration,
                0.0f,
                Tween.EaseInOut,
                Tween.LoopType.PingPong
            );
        }
    }
}