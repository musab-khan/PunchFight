// © 2015 Mario Lelas
using UnityEngine;
using System.Collections;
using System;

namespace MLSpace
{
    /// <summary>
    /// class derived from BallProjectile
    /// adds additional force upon impact
    /// </summary>
    public class HarpoonBallProjectile : BallProjectile
    {
        /// <summary>
        /// additional force. Multiplies hit strength upon impact.
        /// </summary>
        [Tooltip ("Additional force. Multiplies hit strength upon impact.")]
        public float force;


        public override string ToString()
        {
            return "HarpoonBallProjectile";
        }

        // unity physics on collision enter
        void OnCollisionEnter(Collision _collision)
        {
            // stop and disable if hit object is not within colliding layers 
            if(!Utils.DoesMaskContainsLayer (collidingLayers,_collision .collider .gameObject .layer ))
            {
                this.State = ProjectileStates.Done;
                this.RigidBody.velocity = Vector3.zero;
                this.RigidBody.isKinematic = true;
                this.RigidBody.detectCollisions = false;
                this.SphereCollider.enabled = false;
            }
        }

        public override void setup()
        {
            /*
            On hit increase hit force to single bodypart and set ragdoll event time delegate to fire 
            in 5 sec.  to fire getting up animation
            */

            this.OnHit = () =>
            {
                if (this.HitInfo.hitObject)
                {
                    RagdollManager rman = null;
                    Collider col = null;
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

                    rman = ragdollUser.ragdollManager;

                    if (!rman)
                    {
#if DEBUG_INFO
                        Debug.LogError("Ball::OnHit cannot find RagdollManager component on " +
                            this.HitInfo.hitObject.name + ".");
#endif
                        return;
                    }
                    if (!rman.acceptHit)
                    {
                        BallProjectile.DestroyProjectile(this);
                        return;
                    }
                    if (ragdollUser.ignoreHit)
                    {
                        BallProjectile.DestroyProjectile(this);
                        return;
                    }

                    col = this.HitInfo.collider;
                    if (!col)
                    {
#if DEBUG_INFO
                        Debug.Log("Ball::OnHit cannot find collider component on " +
                            this.HitInfo.hitObject.name + ".");
#endif
                        return;
                    }

                    this.RigidBody.isKinematic = true;
                    this.SphereCollider.isTrigger = true;
                    this.RigidBody.detectCollisions = false;
                    this.transform.position = col.bounds.center;
                    this.transform.SetParent(col.transform);

#if DEBUG_INFO
                    if (this.HitInfo.bodyPartIndices == null)
                    {
                        Debug.LogError("object cannot be null.");
                        return;
                    }
#endif

                    float strength = this.CurrentHitStrength * this.force;
                    rman.startRagdoll(this.HitInfo.bodyPartIndices, this.HitInfo.hitDirection * strength,
                        this.HitInfo.hitDirection * strength * 0.45f);

                    ragdollUser.ignoreHit = true;
                    rman.ragdollEventTime = 3.0f;
                    rman.OnTimeEnd = () =>
                    {
                        rman.blendToMecanim();
                        ragdollUser.ignoreHit = false;
                    };
                }
            };
        }

    } 

}
