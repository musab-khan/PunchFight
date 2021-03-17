using System;
using MLSpace;
using UnityEngine;

namespace MLSpace
{ 
    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    [RequireComponent(typeof(ThirdPersonCharacterUnity))]
    public class AICharacterControlCustom : MonoBehaviour, IRagdollUser
    {
        public UnityEngine.AI.NavMeshAgent agent { get; private set; } // the navmesh agent required for the path finding
        public ThirdPersonCharacterUnity character { get; private set; } // the character we are controlling
        public Transform OrientTransform;

        private RagdollManager m_Ragdoll;
        private Collider[] m_Colliders;         // for calculating bounds
        private Bounds m_Bounds;                // bounds of ragdoll user
        private bool m_Initialized = false;     // is compoenent initialized ?
        private CapsuleCollider m_Capsule;      // bound capsule of ragdoll user
        private Rigidbody m_Rigidbody;          // rigid body of ragdoll user
        private Animator m_Animator;            // animator component

        /// <summary>
        /// IRagdollUser interface
        /// gets and sets ignore hits flag
        /// </summary>
        public bool ignoreHit { get; set; }

        /// <summary>
        /// gets bounds of character
        /// </summary>
        public Bounds bounds { get { calculateBounds(); return m_Bounds; } }

        /// <summary>
        /// IRagdollInterface
        /// gets reference to ragdoll manager component
        /// </summary>
        public RagdollManager ragdollManager { get { return m_Ragdoll; } }

        


        public Transform[] targets; // target to aim for
        private int m_CurrentTargetIndex = 0;

        private bool m_Jump = false;
        private bool m_Crouch = false;

        /// <summary>
        /// initialize component
        /// </summary>
        public void Initialize()
        {
            if (m_Initialized) return;

            // get the components on the object we need ( should not be null due to require component so no need to check )
            agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacterUnity>();

            agent.updateRotation = false;
            agent.updatePosition = false;

            if (targets.Length == 0) { Debug.LogError("No waypoints exists."); return; }
            agent.SetDestination(targets[m_CurrentTargetIndex].position);

            m_Capsule = GetComponent<CapsuleCollider>();
            if (!m_Capsule) { Debug.LogWarning("Cannot find capsule collider"); return; }

            m_Rigidbody = GetComponent<Rigidbody>();
            if (!m_Rigidbody) { Debug.LogWarning("Cannot find rigidbody"); return; }

            m_Animator = GetComponent<Animator>();
            if (!m_Animator) { Debug.LogError("Cannot find animator component."); return; }

            m_Ragdoll = GetComponent<RagdollManager>();
            if (!m_Ragdoll) { Debug.LogError("cannot find 'RagdollManager' component."); return; }

            Collider col = GetComponent<Collider>();
            if (!col) { Debug.LogError("object cannot be null."); return; }

            m_Bounds = col.bounds;

            if (!OrientTransform && m_Animator.isHuman)
                OrientTransform = m_Animator.GetBoneTransform(HumanBodyBones.Hips);

            // setup important ragdoll events

            // event that will fire when hit
            m_Ragdoll.OnHit = () =>
            {
                character.simulateRootMotion = false;
                character.DisableMove = true;
                m_Rigidbody.velocity = Vector3.zero;
                m_Rigidbody.detectCollisions = false;
                m_Rigidbody.isKinematic = true;
                character.Capsule.enabled = false;
                agent.enabled = false;
            };

            // allow movement when transitioning to animated
            m_Ragdoll.OnStartTransition = () =>
            {
                /* 
                    Enable simulating root motion on transition  if 
                    character is not in full ragdoll to
                    make character not freeze on place when hit.
                    Otherwise root motion will interfere with getting up animation.
                */
                if (!m_Ragdoll.isFullRagdoll && !m_Ragdoll.isGettingUp)
                {
                    character.simulateRootMotion = true;
                    m_Rigidbody.detectCollisions = true;
                    m_Rigidbody.isKinematic = false;
                    m_Capsule.enabled = true;
                    agent.enabled = true;
                }
            };

            // event that will be last fired ( when full ragdoll - on get up, when hit reaction - on blend end 
            m_Ragdoll.LastEvent = () =>
            {
                character.simulateRootMotion = true;
                character.DisableMove = false;

                m_Rigidbody.detectCollisions = true;
                m_Rigidbody.isKinematic = false;
                m_Capsule.enabled = true;
                agent.enabled = true;
            };

            // event that will be fired when ragdoll counter reach event time
            m_Ragdoll.ragdollEventTime = 3.0f;
            m_Ragdoll.OnTimeEnd = () =>
            {
                m_Ragdoll.blendToMecanim();
            };

            ignoreHit = false;


            m_Initialized = true;
        }

        // Use this for initialization
        private void Start()
        {
            Initialize();
        }

        // Update is called once per frame
        private void Update()
        {
            if (!agent.enabled)
            {
                character.Move(Vector3.zero, false, false);
                return;
            }




            if(Input.GetKeyDown (KeyCode.J))
            {
                m_Jump = true;
            }
            if (Input.GetKey(KeyCode.C))
                m_Crouch = true;

#if DEBUG_INFO

            if (targets == null) { Debug.LogError("object cannot be null."); return; }
            if (targets.Length == 0) { Debug.LogError("No waypoints exists."); return; }
            if (m_CurrentTargetIndex < 0 || m_CurrentTargetIndex >= targets.Length)
            {
                Debug.LogError("target index out of range."); return;
            }
#endif
            float distanceFromTarget = Vector3.Distance(transform.position, targets[m_CurrentTargetIndex].position);
            Vector3 manualVelocity = targets[m_CurrentTargetIndex].position - transform.position;
            manualVelocity.y = 0.0f;
            manualVelocity.Normalize();
            float speed = 60.0f;
            manualVelocity = manualVelocity * Time.deltaTime * speed;

            Debug.DrawLine(targets[m_CurrentTargetIndex].position,
                targets[m_CurrentTargetIndex].position + Vector3.up * 6f, Color.blue);
            Debug.DrawLine(transform.position, transform.position + agent.desiredVelocity, Color.yellow);
            Debug.DrawLine(transform.position, transform.position + manualVelocity, Color.red);


            if (distanceFromTarget < 1.0f)
            {
                m_CurrentTargetIndex++;
                if (m_CurrentTargetIndex >= targets.Length)
                    m_CurrentTargetIndex = 0;
            }

            // use the values to move the character
            character.Move(manualVelocity, m_Crouch , m_Jump);

            m_Jump = false;
            //m_Crouch = false;
        }


        public void SetTargets(Transform[] _targets)
        {
            this.targets = _targets;
        }

        public void SetJump(bool _jump)
        {
            m_Jump = _jump;
        }

        public void SetCrouch(bool _crouch)
        {
            m_Crouch = _crouch;
        }

        /// <summary>
        /// IRagdollUser interface
        /// start hit reaction flag
        /// </summary>
        /// <param name="hitParts">hit body parts</param>
        /// <param name="hitForce">hit velocity</param>
        public void startHitReaction(int[] hitParts, Vector3 hitForce)
        {
#if DEBUG_INFO
            if (!m_Initialized)
            {
                Debug.LogError("component not initialized.");
                return;
            }
#endif
            m_Ragdoll.startHitReaction(hitParts, hitForce);
        }


        /// <summary>
        /// IRagdollUser interface
        /// starts full ragdoll method
        /// </summary>
        /// <param name="bodyParts">hit parts</param>
        /// <param name="bodyPartForce">force on hit parts</param>
        /// <param name="overallForce">force on all parts</param>
        public void startRagdoll(int[] bodyParts, Vector3 bodyPartForce, Vector3 overallForce)
        {
#if DEBUG_INFO
            if (!m_Initialized)
            {
                Debug.LogError("component not initialized.");
                return;
            }
#endif
            m_Ragdoll.startRagdoll(bodyParts, bodyPartForce, overallForce);
        }

        // calculate bounds by combining all ragdoll colliders
        private void calculateBounds()
        {
            if (!m_Ragdoll) m_Ragdoll = GetComponent<RagdollManager>();
#if DEBUG_INFO
            if (!m_Ragdoll) { Debug.LogError("Cannot find RagdollManager component."); return; }
#endif
            if (m_Colliders == null)
                m_Colliders = m_Ragdoll.RagdollBones[0].GetComponentsInChildren<Collider>();

            Vector3 vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 vMax = new Vector3(-float.MaxValue, -float.MaxValue, -float.MaxValue);
            for (int i = 0; i < m_Colliders.Length; i++)
            {
                vMin = Vector3.Min(vMin, m_Colliders[i].bounds.min);
                vMax = Vector3.Max(vMax, m_Colliders[i].bounds.max);
            }
            m_Bounds.SetMinMax(vMin, vMax);
        }
    } 
}
