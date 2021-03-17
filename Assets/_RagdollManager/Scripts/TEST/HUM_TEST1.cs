using UnityEngine;
using MLSpace;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(RagdollManager))]
public class HUM_TEST1 : MonoBehaviour, IRagdollUser
{
    private RagdollManager m_Ragdoll;
    private Collider m_Capsule;
    private Rigidbody m_Rigidbody;
    private Animator m_Animator;
    private ThirdPersonCharacter m_Character;           // this npc character component


    private Bounds m_Bounds;                            // bounds
    private Collider[] m_Colliders;                     // used for calculating bounds

#region Properties

    /// <summary>
    /// IRagdollUser interface
    /// gets and sets falg to ignore hits
    /// </summary>
    public bool ignoreHit { get; set; }

    /// <summary>
    /// gets bounds of character
    /// </summary>
    public Bounds bounds { get { calculateBounds(); return m_Bounds; } }

    /// <summary>
    /// IRagdollUser interface
    /// gets reference to ragdoll manager script
    /// </summary>
    public RagdollManager ragdollManager { get { return m_Ragdoll; } }

#endregion

    // Use this for initialization
    void Start()
    {
        m_Character = GetComponent<ThirdPersonCharacter>();
        if (!m_Character) { Debug.LogError("cannot find 'ThirdPersonCharacter' component."); return; }
        m_Character.Initialize();
        if (!m_Character.Initialized) { Debug.LogError("cannot initialize 'ThirdPersonCharacter' component."); return; }
        m_Bounds = m_Character.Capsule.bounds;
        ignoreHit = false;
        m_Ragdoll = GetComponent<RagdollManager>();

        m_Rigidbody = GetComponent<Rigidbody>();
        if (!m_Rigidbody) { Debug.LogWarning("Cannot find rigidbody"); }
        m_Rigidbody.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ |
            RigidbodyConstraints.FreezeRotationY;

        m_Animator = GetComponent<Animator>();
        if (!m_Animator) { Debug.LogError("Cannot find animator component."); }

        m_Capsule = GetComponent<Collider>();
        if (!m_Capsule) { Debug.LogError("Cannot find Collider component."); }

        m_Ragdoll.OnHit = () =>
        {
            m_Rigidbody.velocity = Vector3.zero;
            m_Rigidbody.detectCollisions = false;
            m_Rigidbody.isKinematic = true;
            m_Capsule.enabled = false;
        };
        m_Ragdoll.LastEvent = () =>
        {
            m_Rigidbody.detectCollisions = true;
            m_Rigidbody.isKinematic = false;
            m_Capsule.enabled = true;
        };
        m_Ragdoll.OnStartTransition = () =>
        {
            if (!m_Ragdoll.isFullRagdoll && !m_Ragdoll.isGettingUp)
            {
                m_Rigidbody.detectCollisions = true;
                m_Rigidbody.isKinematic = false;
                m_Capsule.enabled = true;
            }
        };

        calculateBounds();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            m_Ragdoll.startRagdoll();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            m_Ragdoll.blendToMecanim();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            bool strafe = m_Animator.GetBool("Strafe");
            m_Animator.SetBool("Strafe", !strafe);
        }
        //if (Input.GetMouseButtonDown(0))
        //{
        //    doRagdoll();
        //}
        //if (Input.GetMouseButtonDown(1))
        //{
        //    doHitReaction();
        //}
    }

    private void doHitReaction()
    {
        Ray currentRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        int mask = LayerMask.GetMask("ColliderLayer", "ColliderInactiveLayer");
        RaycastHit rhit;
        if (Physics.Raycast(currentRay, out rhit, 120.0f, mask))
        {
            BodyColliderScript bcs = rhit.collider.GetComponent<BodyColliderScript>();
            int[] parts = new int[] { bcs.index };
            bcs.ParentRagdollManager.startHitReaction(parts, currentRay.direction * 16.0f);
        }
    }

    private void doRagdoll()
    {
        Ray currentRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        int mask = LayerMask.GetMask("ColliderLayer", "ColliderInactiveLayer");
        RaycastHit rhit;
        if (Physics.Raycast(currentRay, out rhit, 120.0f, mask))
        {
            BodyColliderScript bcs = rhit.collider.GetComponent<BodyColliderScript>();
            int[] parts = new int[] { bcs.index };
            bcs.ParentRagdollManager .startRagdoll(parts, currentRay.direction * 16.0f);
        }
    }

    public void startHitReaction(int[] hitParts, Vector3 hitForce)
    {
        m_Ragdoll.startHitReaction(hitParts, hitForce );
    }

    public void startRagdoll(int[] bodyParts, Vector3 bodyPartForce, Vector3 overallForce)
    {
        m_Ragdoll.startRagdoll(bodyParts, bodyPartForce, overallForce);
    }

    // calculate bounds based on all colliders sizes
    private void calculateBounds()
    {
        if (!m_Ragdoll) m_Ragdoll = GetComponent<RagdollManager>();
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
