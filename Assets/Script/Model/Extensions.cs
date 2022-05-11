using UnityEngine;

namespace Model
{
    public static class Extensions
    {
        /// <summary>
        /// Animatorのstateに関するパラメータを設定
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="state"></param>
        public static void SetParameters(this Animator animator, AnimatorState state)
        {
            var controller = animator.GetComponent<EnemyController>();
            switch (state)
            {
                case AnimatorState.Run:
                    animator.SetFloat("MoveSpeed", controller.SpeedX / 2);
                    animator.SetBool("MoveMirror", controller.VelocityX > 0);
                    break;

                case AnimatorState.Jump:
                    break;

                case AnimatorState.Die:
                    animator.SetTrigger("Die");
                    break;

                case AnimatorState.Throw:
                    animator.SetTrigger("Throw");
                    break;
            }
        }
    }

    public enum AnimatorState
    {
        /// <summary>
        /// <para>パラメータ：</para>
        /// <para>float MoveSpeed</para>
        /// <para>bool MoveMirror</para>
        /// </summary>
        Run,

        Jump,

        /// <summary>
        /// <para>パラメータ：</para>
        /// <para>trigger Die</para>
        /// </summary>
        Die,

        Throw
    }
}