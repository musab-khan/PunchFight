using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLSpace
{
    /// <summary>
    /// void delegate that takes BallProjectile class as parameter
    /// </summary>
    /// <param name="thisBall"></param>
    public delegate void ProjectileFunc(ProjectileBase thisProjectile);

    public abstract class ProjectileBase : MonoBehaviour
    {
        /// <summary>
        /// possible ball states enumeration
        /// </summary>
        public enum ProjectileStates { Ready, Fired, Done };


        /// <summary>
        /// projectile ball counter
        /// </summary>
        public static int projectileCount = 0;

        /// <summary>
        /// lifetime of projectile
        /// </summary>
        [Tooltip("Lifetime of fire ball projectile.")]
        public float lifetime = 12.0f;

        /// <summary>
        /// hit strength of fire ball projectile
        /// </summary>
        [Tooltip("Hit strength of fire ball projetile.")]
        public float hitStrength = 25.0f;

        protected float m_CurrentLifetime = 0.0f;                 // lifetime of fire ball
        protected float m_CurrentHitStrength = 25.0f;             // current hit strength of fire ball
        protected Rigidbody m_Rigidbody;                          // reference to rigid body 
        protected GameObject m_Owner;                     // owner character of this fireball ( to ignore hits if wanted)
        protected ProjectileStates m_State = ProjectileStates.Ready;  // current state of fire ball
        protected Collider m_Collider;                // sphere collider component
        protected ProjectileFunc m_OnLifetimeExpire = null;

        /// <summary>
        /// gets current life time
        /// </summary>
        public float CurrentLifetime { get { return m_CurrentLifetime; } }

        /// <summary>
        /// gets owner object of projectile
        /// </summary>
        public GameObject Owner { get { return m_Owner; } }

        /// <summary>
        /// gets reference to rigidbody component
        /// </summary>
        public Rigidbody RigidBody
        {
            get
            {
#if DEBUG_INFO
                if (!m_Rigidbody) { Debug.LogError("object cannot be null."); return null; }
#endif
                return m_Rigidbody;
            }
        }

        /// <summary>
        /// gets reference to sphere collider component
        /// </summary>
        public Collider Collider { get { return m_Collider; } }

        

        /// <summary>
        /// gets and sets on lifetime expire event
        /// </summary>
        public ProjectileFunc OnLifetimeExpire { get { return m_OnLifetimeExpire; } set { m_OnLifetimeExpire = value; } }


        /// <summary>
        /// gets and sets state of fire ball
        /// </summary>
        public ProjectileStates State { get { return m_State; } set { m_State = value; } }

        /// <summary>
        /// gets and sets current hit strength
        /// </summary>
        public float CurrentHitStrength { get { return m_CurrentHitStrength; } set { m_CurrentHitStrength = value; } }


        public virtual bool initialize()
        {
            m_Collider = GetComponent<Collider>();
            if (!m_Collider) { Debug.LogError("'Collider' component missing."); return false; }
            m_Rigidbody = GetComponent<Rigidbody>();
            if (!m_Rigidbody) { Debug.LogError("cannot find 'Rigidbody' component."); return false; }
            m_CurrentHitStrength = hitStrength;
            return true;
        }

        // virtual update for derived classes
        protected virtual void update()
        {
            Vector3 transformPosition = transform.position;

            // advance lifetime starting from time when fired onwards
            if (m_State != ProjectileStates.Ready)
            {
                m_CurrentLifetime += Time.deltaTime;
                if (m_CurrentLifetime > lifetime)
                {

                    if (m_OnLifetimeExpire != null)
                    {
                        m_OnLifetimeExpire(this);
                    }
                    else
                    {
                        Destroy(this.gameObject);
                    }
                    return;
                }
            }
        }

        public abstract void setup();

        /// <summary>
        /// Creates ball projectile, assignes owner and increments ball counter
        /// </summary>
        /// <param name="prefab">prefab of projectile</param>
        /// <param name="xform">position and rotation transform</param>
        /// <param name="_owner">owner game object</param>
        /// <returns>created ball projectile</returns>
        public static ProjectileBase CreateProjectile(ProjectileBase prefab, Transform xform, GameObject _owner)
        {
#if DEBUG_INFO
            if (!prefab) { Debug.LogError("object cannot be null."); return null; }
#endif
            projectileCount++;
            if (xform)
            {
                ProjectileBase newBall = (ProjectileBase)Instantiate(prefab,
                   xform.position,
                   xform.rotation);
                newBall.name = newBall.name + ProjectileBase.projectileCount;
                newBall.m_Owner = _owner;
                return newBall;
            }
            else
            {
                ProjectileBase newBall = (ProjectileBase)Instantiate(prefab);
                newBall.name = newBall.name + ProjectileBase.projectileCount;
                newBall.m_Owner = _owner;
                return newBall;
            }
        }

        /// <summary>
        /// destroys ball projectile and decrements counter
        /// </summary>
        /// <param name="ball"></param>
        public static void DestroyProjectile(ProjectileBase projectile)
        {
            if (!projectile) return;

            ProjectileBase.projectileCount--;
            Destroy(projectile.gameObject);
        }


    } 
}
