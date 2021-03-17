
using UnityEngine;
using System.Collections;
using System;

namespace MLSpace
{
    public class InflatableBall : BallProjectile
    {
        /// <summary>
        /// start scale of projectile ball
        /// </summary>
        [Tooltip("Starting scale of projectile ball.")]
        public static float MinScale = 0.15f;

        /// <summary>
        /// max scale of projectile ball
        /// </summary>
        [Tooltip("Maximum scale of projectile ball.")]
        public static float MaxScale = 1.0f;

        private float m_CurrentBallScale = 0.1f;      // current ball scale
        public float CurrentBallScale { get { return m_CurrentBallScale; } set { m_CurrentBallScale = value; } }

        public override string ToString()
        {
            return "InflatableBall";
        }

        public void inflate(float inflateValue = 0.01f)
        {
            m_CurrentBallScale += inflateValue;
            m_CurrentBallScale = Mathf.Min(m_CurrentBallScale, InflatableBall.MaxScale);
            transform.localScale = Vector3.one * m_CurrentBallScale;
            CurrentHitStrength = hitStrength * m_CurrentBallScale;
        }

        public override void setup()
        {
            this.OnHit = () =>
            {
                if (this.HitInfo.hitObject)
                {
                    RagdollManager ragMan = null;
                    IRagdollUser ragdollUser = null;
                    ragdollUser = this.HitInfo.hitObject.GetComponent<IRagdollUser>();
                    if (ragdollUser == null)
                    {
#if DEBUG_INFO
                        Debug.LogError("Ball::OnHit cannot find ragdoll user object on " +
                            this.HitInfo.hitObject.name + ".");
#endif
                        return;
                    }
                    ragMan = ragdollUser.ragdollManager;

                    if (!ragMan)
                    {
#if DEBUG_INFO
                        Debug.LogError("Ball::OnHit cannot find RagdollManager component on " +
                            this.HitInfo.hitObject.name + ".");
#endif
                        return;
                    }

                    if (!ragMan.acceptHit)
                    {
                        return;
                    }

                    //if (ragdollUser.IgnoreHit)
                    //{
                    //    BallProjectile.DestroyBall(ball);
                    //    return;
                    //}

                    Vector3 force = this.HitInfo.hitDirection * this.CurrentHitStrength;
                    ragMan.startHitReaction(this.HitInfo.bodyPartIndices, force);
                }

            };
        }

        public static void Setup(InflatableBall ball)
        {
            ball.OnHit = () =>
            {
                if (ball.HitInfo.hitObject)
                {
                    RagdollManager ragMan = null;
                    IRagdollUser ragdollUser = null;
                    ragdollUser = ball.HitInfo.hitObject.GetComponent<IRagdollUser>();
                    if (ragdollUser == null)
                    {
#if DEBUG_INFO
                        Debug.LogError("Ball::OnHit cannot find ragdoll user object on " +
                            ball.HitInfo.hitObject.name + ".");
#endif
                        return;
                    }
                    ragMan = ragdollUser.ragdollManager;

                    if (!ragMan)
                    {
#if DEBUG_INFO
                        Debug.LogError("Ball::OnHit cannot find RagdollManager component on " +
                            ball.HitInfo.hitObject.name + ".");
#endif
                        return;
                    }

                    if (!ragMan.acceptHit)
                    {
                        return;
                    }

                    //if (ragdollUser.IgnoreHit)
                    //{
                    //    BallProjectile.DestroyBall(ball);
                    //    return;
                    //}

                    Vector3 force = ball.HitInfo.hitDirection * ball.CurrentHitStrength;
                    ragMan.startHitReaction(ball.HitInfo.bodyPartIndices, force);
                }

            };
        }

    } 
}
