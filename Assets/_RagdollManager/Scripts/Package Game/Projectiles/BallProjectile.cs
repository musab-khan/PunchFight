// © 2015 Mario Lelas

//#if DEBUG_INFO
//#define DEBUG_DRAW
//#endif

using UnityEngine;
using System.Collections.Generic;

namespace MLSpace
{



    /// <summary>
    ///  Base Projectile class 
    ///  Checking for collision between set collider layer by 
    ///  checking spherecast from last postion to current
    /// </summary>
    [RequireComponent (typeof(SphereCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public abstract class BallProjectile : ProjectileBase
    {

        private SphereCollider m_SphereCollider;                // sphere collider component

        /// <summary>
        /// gets reference to sphere collider component
        /// </summary>
        public SphereCollider SphereCollider { get { return m_SphereCollider; } }

        public override string ToString()
        {
            return "BallProjectile";
        }


        /// <summary>
        /// Information on hit object 
        /// </summary>
        public class HitInformation
        {
            public GameObject hitObject = null;
            public Collider collider = null;
            public int[] bodyPartIndices = null;
            public Vector3 hitDirection = Vector3.zero;
            public float hitStrength = 0.0f;
        }

        /// <summary>
        /// colliding layers of fire ball. Use 'ColliderLayer'
        /// </summary>
        [Tooltip("colliding layers of fire ball.")]
        public LayerMask collidingLayers;


        protected HitInformation m_HitInfo = null;        // information on hit object
        protected VoidFunc m_OnHit = null;                // on hit delegate

        protected Vector3? m_LastPosition = null;



        /// <summary>
        /// gets information on hit object
        /// </summary>
        public HitInformation HitInfo { get { return m_HitInfo; } }

        /// <summary>
        /// gets and sets on hit event
        /// </summary>
        public VoidFunc OnHit { get { return m_OnHit; } set { m_OnHit = value; } }

        /// <summary>
        /// initialize component
        /// </summary>
        /// <returns>is initialization success</returns>
        public override bool initialize()
        {
            m_SphereCollider = GetComponent<SphereCollider>();
            if (!m_SphereCollider) { Debug.LogError("SphereCollider component missing."); return false; }
            m_Collider = m_SphereCollider;

            m_Rigidbody = GetComponent<Rigidbody>();
            if (!m_Rigidbody) { Debug.LogError("cannot find 'Rigidbody' component."); return false; }

            m_HitInfo = new HitInformation();

            return true;
        }

        // Unity MonoBehaviour start method
        void Start()
        {
            if (!initialize()) { Debug.LogError("cannot initialize component."); return; }
        }

        // Unity MonoBehaviour LateUpdate method
        void LateUpdate()
        {
#if DEBUG_INFO
            if (!RigidBody) { Debug.LogError("object cannot be null."); return; }
#endif
            update();
        }

        // virtual update for derived classes
        protected override void update()
        {
            spherecast();
            Vector3 transformPosition = transform.position;
            base.update();
            m_LastPosition = transformPosition;
        }

        
        protected void spherecast()
        {
            Vector3 transformPosition = transform.position;

            
            
#if DEBUG_DRAW
            positionList.Add(transformPosition);
            radiusList.Add(m_SphereCollider.radius * this.transform.localScale.x);
#endif
            // check for collision only when fired
            if (m_State == ProjectileStates.Fired && m_LastPosition .HasValue )
            {

                // shoot sphere from last position to current 
                // and check if we have a hit

                int mask = collidingLayers;


#if DEBUG_INFO
                if (!m_SphereCollider)
                {
                    Debug.LogError("SphereCollider missing.");
                    return;
                }
#endif

                float radius = m_SphereCollider.radius * this.transform.localScale.x;
                Vector3 difference = transformPosition - m_LastPosition.Value ;
                Vector3 direction = difference.normalized;
                float length = difference.magnitude;
                Vector3 rayPos = m_LastPosition.Value;


                Ray ray = new Ray(rayPos, direction);

                RaycastHit[] hits = Physics.SphereCastAll(ray, radius, length, mask);

                List<int> chosenHits = new List<int>();
                RagdollManager ragMan = null;

                RaycastHit? rayhit = null;

                for (int i = 0; i < hits.Length; i++)
                {
                    RaycastHit rhit = hits[i];
                    BodyColliderScript bcs = rhit.collider.GetComponent<BodyColliderScript>();
                    if (!bcs)
                    {
#if DEBUG_INFO
                        Debug.LogError("BodyColliderScript missing on " + rhit.collider.name);
#endif
                        continue;
                    }

                    if (!bcs.ParentObject)
                    {
#if DEBUG_INFO
                        Debug.LogError("BodyColliderScript.ParentObject missing on " + rhit.collider.name);
#endif
                        continue;
                    }
                    if (bcs.ParentObject == this.m_Owner)
                    {
                        continue;
                    }

                    if(!ragMan)
                    {
                        ragMan = bcs.ParentRagdollManager;
                        m_HitInfo.hitObject = bcs.ParentObject;
                        m_HitInfo.collider = rhit.collider;
                        m_HitInfo.hitDirection = direction;
                        m_HitInfo.hitStrength = m_CurrentHitStrength;
                        rayhit = rhit;
                    }

                    chosenHits.Add(bcs.index);
                }


                if (hits.Length > 0)
                {
                    if(ragMan)
                    {

                        if (!rayhit.HasValue)
                        {
#if DEBUG_INFO
                            Debug.LogError("object cannot be null.");
#endif
                            return;
                        }

                        Vector3 n = rayhit.Value.normal;
                        Vector3 r = Vector3.Reflect(direction, n);
                        this.transform.position = rayPos + ray.direction *
                            (rayhit.Value.distance - radius);
                        Vector3 vel = r;
                        this.m_Rigidbody.velocity = vel;

                        Vector3 force = direction * m_CurrentHitStrength;
                        m_State = ProjectileStates.Done;

                        m_HitInfo.bodyPartIndices = chosenHits.ToArray();

                        if (m_OnHit != null)
                            m_OnHit();
                        else
                        {
                            ragMan.startHitReaction(m_HitInfo.bodyPartIndices, force);
                        }
                    }
#if DEBUG_INFO
                    else
                    {
                        BodyColliderScript bcs = hits[0].collider.GetComponent<BodyColliderScript>();
                        if (!bcs)
                            return;
                        if (!bcs.ParentObject)
                            return;
                        if (bcs.ParentObject == this.m_Owner)
                            return;
                        Debug.LogWarning("RagdollUser interface not implemented. " +
                        bcs.ParentObject.name);
                    }
#endif
                }

            }

        }






#if DEBUG_DRAW
        List<Vector3> positionList = new List<Vector3>();
        List<float> radiusList = new List<float>();
        void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            for(int i = 0;i<positionList .Count-1;i++)
            {
                Gizmos.DrawWireSphere(positionList[i], radiusList[i]);
                Gizmos.DrawLine(positionList[i],positionList[i + 1]);
            }
        }
#endif

    } 
}
