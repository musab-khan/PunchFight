// © 2016 Mario Lelas
using UnityEngine;

namespace MLSpace
{
    /// <summary>
    /// Put this script on any object you want to shoot from.
    /// Point it towards direction to which you want to shoot.
    /// Uses spherecasting as collision detection
    /// </summary>
    public class ShootScript : MonoBehaviour
    {
        [Tooltip("Shoot projectile prefab")]
        public ProjectileBase ProjectilePrefab;

        [Tooltip ("Projectile fire position and shoot direction.")]
        public Transform FireTransform;

        [Tooltip("Owner character of shooter script.")]
        public GameObject Owner;

        [HideInInspector]
        public ProjectileBase m_CurrentProjectile = null;        // current ball reference

        [HideInInspector]
        public bool m_DisableShooting = false;              // disable shooting flag


        private bool m_Fire = false, m_FireDown = false, m_FireUp = false;



        void Start()
        {
            initialize();
        }

        public virtual void initialize()
        {
            if (!ProjectilePrefab) { Debug.LogError("projectile prefab not assigned."); return; }
            if (!FireTransform) { Debug.LogError("fire transform not assign."); return; }
        }

        // create ball projectile function
        protected virtual void createProjectile()
        {
            if (m_CurrentProjectile) return;

            m_CurrentProjectile = ProjectileBase.CreateProjectile(ProjectilePrefab, FireTransform, Owner);
            if (!m_CurrentProjectile.initialize()) { Debug.LogError("cannot initialize ball projectile"); return; }
            m_CurrentProjectile.OnLifetimeExpire = ProjectileBase.DestroyProjectile;
            m_CurrentProjectile.Collider.isTrigger = true;
            m_CurrentProjectile.RigidBody.isKinematic = true;
            m_CurrentProjectile.RigidBody.detectCollisions = false;
            ProjectileBase thisBall = m_CurrentProjectile;
            thisBall.setup();
        }

        // scale current ball
        protected virtual void scaleBall()
        {
            if (!m_CurrentProjectile) { return; }
            if (!(m_CurrentProjectile is InflatableBall)) return;
            (m_CurrentProjectile as InflatableBall).inflate();
        }

        // shoot current ball
        protected virtual void fireBall()
        {
            if (!m_CurrentProjectile) { return; }

            Vector3 force = FireTransform.forward * m_CurrentProjectile.hitStrength;
            m_CurrentProjectile.RigidBody.isKinematic = false;
            m_CurrentProjectile.Collider.isTrigger = false;
            m_CurrentProjectile.RigidBody.detectCollisions = true;
            m_CurrentProjectile.RigidBody.velocity = force;
            m_CurrentProjectile.transform.position = FireTransform.position;
            m_CurrentProjectile.State = BallProjectile.ProjectileStates.Fired;
            m_CurrentProjectile = null;
        }

        void FixedUpdate()
        {
            if (m_DisableShooting) return;
            if (ProjectilePrefab is InflatableBall)
            {
                if (m_FireDown)
                {
                    createProjectile();
                    m_FireDown = false;
                }
                if (m_Fire)
                {
                    scaleBall();
                }
                if (m_FireUp)
                {
                    fireBall();
                    m_FireUp = false;
                }
            }
            else
            {
                if (m_FireDown)
                {
                    createProjectile();
                    fireBall();
                    m_FireDown = false;
                }
            }
            if (m_CurrentProjectile)
            {
                if (m_CurrentProjectile.State == BallProjectile.ProjectileStates.Ready)
                    m_CurrentProjectile.transform.position = FireTransform.position;
            }
        }

        // Update is called once per frame
        void LateUpdate()
        {
#if DEBUG_INFO
            if (!ProjectilePrefab)
            {
                Debug.LogError("ProjectilePrefab cannot be null.");
                return;
            }
#endif

            if (Input.GetButtonDown("Fire1"))
                m_FireDown = true;
            if (Input.GetButtonUp("Fire1"))
                m_FireUp = true;

            m_Fire = Input.GetButton("Fire1");
        }
    } 

}
