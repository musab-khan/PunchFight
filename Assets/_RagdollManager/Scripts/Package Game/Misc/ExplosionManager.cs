// © 2017 Mario Lelas
using UnityEngine;
using System.Collections.Generic;

namespace MLSpace
{
    /// <summary>
    /// Explosion manager. Responsible for creating pool of explosion effects, starting explosions, returning them to pool and updating lifetime.
    /// </summary>
    public class ExplosionManager : MonoBehaviour
    {
        /// <summary>
        /// Explosion particle system effect prefab.
        /// </summary>
        public Explosion explosionPrefab;

        /// <summary>
        /// Lifetime of explosion before returning to pool.
        /// </summary>
        public float explosionLifetime = 5.0f;

        /// <summary>
        /// Explosion sound effects
        /// </summary>
        public AudioClip[] explosionSounds;



        private AudioSource m_ASource;      // reference to audio source component
        private List<Explosion> explosionPool;  // effects pool
        private bool m_Initialized = false;     // is component initialized

        /// <summary>
        /// Unity Start method
        /// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time
        /// </summary>
        private void Start()
        {
            initialize();
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            initialize();
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// OnDestroy will only be called on game objects that have previously been active.
        /// </summary>
        private void OnDestroy()
        {
            clear();
        }

        /// <summary>
        /// Unity Update method
        /// Update is called every frame, if the MonoBehaviour is enabled
        /// </summary>
        public void Update()
        {
#if DEBUG_INFO
            if(!m_Initialized)
            {
                Debug.LogError("Component not initialized: " + this.ToString());
                return;
            }
#endif
            if (Time.timeScale == 0.0f) return;

            int i;
            for(i=explosionPool.Count - 1;i>=0;i--)
            {
                Explosion exp = explosionPool[i];

                if (!exp) { continue; }

                exp.lifeTimer += Time.deltaTime;
                if (exp.lifetime < exp.lifeTimer)
                {
                    exp.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Initialize component
        /// </summary>
        public void initialize()
        {
            if (m_Initialized) return;

            if(explosionPrefab == null)
            {
                Debug.LogError("Explosion prefab not assigned");
                return;
            }
            explosionPool = new List<Explosion>();
            m_ASource = GetComponent<AudioSource>();


            m_Initialized = true;
        }

        /// <summary>
        /// Clear references
        /// </summary>
        public void clear()
        {
            if (explosionPool != null)
            {
                int i;
                for (i = explosionPool.Count - 1; i >= 0; i--)
                {
                    if (explosionPool[i])
                    {
                        if (explosionPool[i].gameObject)
                            Destroy(explosionPool[i].gameObject);
                    }
                    explosionPool[i] = null;
                }
                explosionPool.Clear();
                explosionPool = null;
            }
            m_Initialized = false;
        }

        /// <summary>
        /// Activate and get available explosion.
        /// </summary>
        /// <returns></returns>
        public Explosion getAvailableExplosion()
        {
            int i;
            for (i = 0; i < explosionPool.Count; i++)
            {
                Explosion exp = explosionPool[i];
                if (!exp.gameObject.activeSelf)
                {
                    exp.lifetime = explosionLifetime;
                    exp.lifeTimer = 0.0f;
                    exp.gameObject.SetActive(true);
                    return exp;
                }
            }
            Explosion newExp = (Explosion)Instantiate<Explosion>(explosionPrefab);
            newExp.lifetime = explosionLifetime;
            newExp.lifeTimer = 0.0f;
            explosionPool.Add(newExp);
            return newExp;
        }

        /// <summary>
        /// Start explosion at position
        /// </summary>
        /// <param name="atPosition">position to start explosion</param>
        /// <param name="multiplier">explosion multiplier</param>
        /// <param name="force">explosion force</param>
        /// <returns>explosion</returns>
        public Explosion explodeAt(Vector3 atPosition,float radius,float multiplier = 1.0f, float force = 4.0f)
        {
#if DEBUG_INFO
            if (!m_Initialized)
            {
                Debug.LogError("Component not initialized: " + this.ToString());
                return null;
            }
#endif
            GameUtils.PlayRandomClip(m_ASource, explosionSounds);

            int i;
            for (i = 0;i<explosionPool .Count;i++)
            {
                Explosion exp = explosionPool[i];
                if(!exp.gameObject.activeSelf )
                {
                    exp.transform.position = atPosition;
                    exp.lifetime = explosionLifetime;
                    exp.lifeTimer = 0.0f;
                    exp.gameObject.SetActive(true);
                    exp.startExplosion(radius,multiplier, force);
                    return exp;
                }
            }
            Explosion newExp = (Explosion)Instantiate<Explosion>(explosionPrefab);
            newExp.transform.position = atPosition;
            newExp.lifetime = explosionLifetime;
            newExp.lifeTimer = 0.0f;
            explosionPool.Add(newExp);
            newExp.startExplosion(radius, multiplier, force);
            return newExp;
        }

        /// <summary>
        /// Start explosion.
        /// </summary>
        /// <param name="explosion"></param>
        /// <param name="radius"></param>
        /// <param name="multiplier"></param>
        /// <param name="force"></param>
        public void explodeAt(Explosion explosion,float radius,float multiplier = 1.0f,float force = 4.0f)
        {
            GameUtils.PlayRandomClip(m_ASource, explosionSounds);
            explosion.startExplosion(radius, multiplier, force);
        }

    }

}