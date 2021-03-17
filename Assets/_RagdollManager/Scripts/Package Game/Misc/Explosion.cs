// © 2016 Mario Lelas modified Unity standard assets
using UnityEngine;
using System.Collections.Generic;

namespace MLSpace
{
    public class Explosion : MonoBehaviour
    {
        /// <summary>
        /// Timer of lifetime of explosion
        /// </summary>
        [HideInInspector]
        public float lifeTimer = 0.0f;

        /// <summary>
        /// Life duration of explosion
        /// </summary>
        [HideInInspector]
        public float lifetime = 5.0f;

        /// <summary>
        /// Assign collision layer of explosion to prevent applying force to items behind walls.
        /// </summary>
        public LayerMask explosionMask;


        /// <summary>
        /// Start explosition at position
        /// </summary>
        /// <param name="multiplier">radius multiplier of objects to collect</param>
        /// <param name="force">explosion force</param>
        public void startExplosion(float radius, float multiplier, float force)
        {
            Vector3 transformPosition = transform.position;

            int mask = explosionMask.value;

            var systems = GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem system in systems)
            {
                ParticleSystem.MainModule main = system.main;
                main.startLifetimeMultiplier *= Mathf.Lerp(1, multiplier, 0.5f);
                //system.startLifetime *= Mathf.Lerp(1, multiplier, 0.5f);
                system.Clear();
                system.Play();
            }

            var cols = Physics.OverlapSphere(transform.position, radius, mask);
            var rigidbodies = new List<Rigidbody>();

            Dictionary<RagdollManager, List<BodyColliderScript>> ragdollData = new Dictionary<RagdollManager, List<BodyColliderScript>>();

            foreach (var col in cols)
            {
                if (!col.attachedRigidbody) { continue; }

                Vector3 pos = transform.position;
                Vector3 dest = col.transform.position; 

                bool reachable = false;
                RaycastHit hit;
                if(Physics.Linecast(pos, dest,out hit, mask))
                {
                    if (hit.collider == col)
                        reachable = true;
                }

                if (reachable)
                {
                    
                    BodyColliderScript bcs = col.GetComponent<BodyColliderScript>();
                    if (bcs)
                    {
                        if (ragdollData.ContainsKey(bcs.ParentRagdollManager))
                        {
                            ragdollData[bcs.ParentRagdollManager].Add(bcs);
                        }
                        else
                        {
                            ragdollData[bcs.ParentRagdollManager] = new List<BodyColliderScript>();
                            ragdollData[bcs.ParentRagdollManager].Add(bcs);
                        }
                    }
                    else
                    {
                        RagdollManager ragMan = col.GetComponent<RagdollManager>();
                        if (ragMan)
                        {
                            ragdollData[ragMan] = null;
                        }
                        else
                        {
                            if (!rigidbodies.Contains(col.attachedRigidbody))
                            {
                                rigidbodies.Add(col.attachedRigidbody);
                            }
                        }
                    }
                }
            }
            foreach (var rb in rigidbodies)
            {
                rb.AddExplosionForce(force * multiplier, transform.position, radius, multiplier, ForceMode.Impulse);
            }

            

            foreach(KeyValuePair <RagdollManager ,List<BodyColliderScript >> pair in ragdollData )
            {
                float dist = Vector3.Distance(transformPosition, pair.Key.transform.position);
                float distMod = 1 - Mathf.Clamp01(dist / radius);
                float forceMod = force * Mathf.Max(distMod, 0.25f);

                Vector3 direction = pair.Key.transform.position - transformPosition;
                direction += Vector3.up * forceMod * 0.5f;
                direction.Normalize();
                Vector3 velocity = direction * forceMod;
                int[] parts = null;
                if (pair.Value != null && pair.Value.Count > 0)
                {
                    parts = new int[pair.Value.Count];
                    for (int i = 0; i < pair.Value.Count; i++)
                        parts[i] = pair.Value[i].index;
                }
                pair.Key.startRagdoll(parts, velocity, velocity * 0.5f);
            }

        }
    } 
}
