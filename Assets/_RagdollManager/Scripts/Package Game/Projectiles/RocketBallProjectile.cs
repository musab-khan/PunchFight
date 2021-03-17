using UnityEngine;
using System.Collections;
using System;

namespace MLSpace
{
    public class RocketBallProjectile : BallProjectile
    {
        public float up_force = 18.0f;

        private Transform m_Afterburner;

        public override bool initialize()
        {
            bool r = base.initialize();
            if (!r) { Debug.LogError("Failed ti initialize base class."); return false; }

            m_Afterburner = transform.Find("Afterburner");
            if(!m_Afterburner) { Debug.LogError("cannot find child 'Afterburner' " + this.name );return false; }
            m_Afterburner.gameObject.SetActive(false);

            return r;
        }

        public override string ToString()
        {
            return "RocketBallProjectile";
        }

        public void startAfterburner()
        {
#if DEBUG_INFO
            if (!m_Afterburner) { Debug.LogError("object cannot be null"); return; }
#endif
            m_Afterburner.gameObject.SetActive(true);
            m_Afterburner.transform.LookAt(m_Afterburner.transform.position + Vector3.down);
        }

        protected override void update()
        {
#if DEBUG_INFO
            if (!m_Afterburner) { Debug.LogError("object cannot be null"); return; }
#endif
            base.update();
            m_Afterburner.transform.LookAt(m_Afterburner.transform.position + Vector3.down);
        }

        public override void setup()
        {
            /*
            On hit start lifting character up by adding extra force to single bodypart and set ragdoll event time delegate to fire 
            in 5 sec. then start another ragdoll removing extra force to make character fall down again.
            also set another timed event to fire getting up animation
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

                    Vector3 boundsSize = col.bounds.size;
                    float max = boundsSize.x;
                    if (boundsSize.y > max) max = boundsSize.y;
                    if (boundsSize.z > max) max = boundsSize.z;



                    this.transform.localScale = new Vector3(max, max, max);
                    this.RigidBody.isKinematic = true;
                    this.SphereCollider.isTrigger = true;
                    this.RigidBody.detectCollisions = false;
                    this.transform.position = col.bounds.center;
                    this.transform.SetParent(col.transform);
                    this.startAfterburner();

#if DEBUG_INFO
                    if (this.HitInfo.bodyPartIndices == null)
                    {
                        Debug.LogError("object cannot be null.");
                        return;
                    }
#endif

                    rman.startRagdoll(this.HitInfo.bodyPartIndices,
                        this.HitInfo.hitDirection * this.HitInfo.hitStrength);


                    Vector3 v = new Vector3(0.0f, this.up_force, 0.0f);
                    RagdollManager.BodyPartInfo b = rman.getBodyPartInfo(
                        this.HitInfo.bodyPartIndices[0]);
                    b.extraForce = v;

                    ragdollUser.ignoreHit = true;
                    rman.ragdollEventTime = 3.0f;
                    rman.OnTimeEnd = () =>
                    {
                        rman.startRagdoll(null, Vector3.zero, Vector3.zero, true);

                        b = rman.getBodyPartInfo(this.HitInfo.bodyPartIndices[0]);
                        b.extraForce = Vector3.zero;

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
