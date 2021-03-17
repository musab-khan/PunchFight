using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLSpace
{
    public class RigidBodyBullet : ProjectileBase
    {
        private void Start()
        {
            initialize();
        }

        private void OnEnable()
        {
            initialize();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (m_State != ProjectileStates.Fired) return;
            collisionFunc(collision);
            m_State = ProjectileStates.Done;
        }

        private void collisionFunc(Collision collision)
        {
           
            BodyColliderScript bcs = collision.collider.GetComponent<BodyColliderScript>();
            if(bcs)
            {

                int[] indices = new int[] { bcs.index };
                //bcs.ParentRagdollManager.startRagdoll(indices, transform.forward * hitForce);
                bcs.ParentRagdollManager.startHitReaction(indices, transform.forward * m_CurrentHitStrength);
                
            }
        }

        public override  void setup()
        {
        }
    } 
}
