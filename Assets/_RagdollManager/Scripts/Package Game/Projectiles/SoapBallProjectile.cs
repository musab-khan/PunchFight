// © 2015 Mario Lelas


using System;
using UnityEngine;

namespace MLSpace
{
    /// <summary>
    /// derived cball projectile class 
    /// </summary>
    public class SoapBallProjectile : BallProjectile
    {
        public float up_force = 1.0f;

        public override string ToString()
        {
            return "SoapBallProjectile";
        }

        public override void setup()
        {
            /*
On hit start lifting character up by adding extra force on all bodyparts and set ragdoll event time delegate to fire 
in 5 sec. then start another ragdoll removing extra force to make character fall down again.
also set another timed event to fire getting up animation
*/

            this.OnHit = () =>
            {
                if (this.HitInfo.hitObject)
                {
                    RagdollManager rman = null;
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


                    Vector3 boundsSize = ragdollUser.bounds.size;
                    float max = boundsSize.x;
                    if (boundsSize.y > max) max = boundsSize.y;
                    if (boundsSize.z > max) max = boundsSize.z;

                    this.transform.localScale = new Vector3(max, max, max);
                    this.RigidBody.isKinematic = true;
                    this.SphereCollider.isTrigger = true;
                    this.RigidBody.detectCollisions = false;
                    this.transform.position = ragdollUser.bounds.center;
                    this.transform.SetParent(rman.rootTransform, true);

                    rman.startRagdoll(null, null/*Vector3.zero*/, Vector3.zero, true);

                    Vector3 v = new Vector3(0.0f, this.up_force, 0.0f);
                    for (int i = 0; i < (int)BodyParts.BODY_PART_COUNT; i++)
                    {
                        RagdollManager.BodyPartInfo b = rman.getBodyPartInfo(i);
                        b.extraForce = v;
                    }

                    ragdollUser.ignoreHit = true;

                    rman.ragdollEventTime = 3.0f;
                    rman.OnTimeEnd = () =>
                    {
                        for (int i = 0; i < (int)BodyParts.BODY_PART_COUNT; i++)
                        {
                            RagdollManager.BodyPartInfo b = rman.getBodyPartInfo(i);
                            b.extraForce = Vector3.zero;
                        }

                        rman.startRagdoll(null, Vector3.zero, Vector3.zero, true);

                        BallProjectile.DestroyProjectile(this);

                        rman.ragdollEventTime = 5.0f;
                        rman.OnTimeEnd = () =>
                        {
                            rman.blendToMecanim();
                        };
                        ragdollUser.ignoreHit = false;
                    };
                }
            };
        }
    } 
}
