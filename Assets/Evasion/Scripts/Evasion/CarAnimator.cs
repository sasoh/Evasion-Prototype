using Pixelplacement;
using UnityEngine;

namespace Evasion
{
    public static class CarAnimator
    {
        public static void Animate(
            Transform transformToReposition,
            GameObject transformToRotate,
            Vector3 currentNodePosition,
            Vector3 targetNodePosition,
            bool isInit
        )
        {
            var targetDirection = (targetNodePosition - currentNodePosition).normalized;
            var facingDirection = -transformToRotate.transform.up; // current sprite points downwards
            var angle = Vector3.SignedAngle(targetDirection, facingDirection, Vector3.back);

            Tween.Rotate(
                transformToRotate.transform,
                new Vector3(0.0f, 0.0f, angle),
                Space.Self,
                AnimationProperties.RotationTweenDuration,
                0.0f
            );

            Tween.Position(
                transformToReposition,
                targetNodePosition,
                isInit ? 0.0f : AnimationProperties.MovementTweenDuration,
                0.0f
            );
        }
    }
}