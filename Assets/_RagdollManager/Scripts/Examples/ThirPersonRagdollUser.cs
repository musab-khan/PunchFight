
using UnityEngine;
using MLSpace;

/// <summary>
/// Example of collision of Rigidbody objet with character with rigidbody and capsule collider componets
/// </summary>
public class ThirPersonRagdollUser : MonoBehaviour
{
    private RagdollManager m_Ragdoll;
    private Collider m_Capsule;
    private Rigidbody m_Rigidbody;
    private Animator m_Animator;



    // Use this for initialization
    void Start()
    {
        m_Ragdoll = GetComponent<RagdollManager>();
        if (!m_Ragdoll) { Debug.LogError("cannot find 'RagdollManager' component."); return; }


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

        // Disable capsule and rigidbody
        m_Ragdoll.OnHit = () =>
        {
            m_Rigidbody.velocity = Vector3.zero;
            m_Rigidbody.detectCollisions = false;
            m_Rigidbody.isKinematic = true;
            m_Capsule.enabled = false;
        };

        // reenable capsule and rigidbody
        m_Ragdoll.LastEvent = () =>
        {
            m_Rigidbody.detectCollisions = true;
            m_Rigidbody.isKinematic = false;
            m_Capsule.enabled = true;
        };

        // reenable rigidbody and capsule on start transition if character is under hit reaction
        // not if its full ragdoll
        m_Ragdoll.OnStartTransition = () =>
        {
            if (!m_Ragdoll.isFullRagdoll && !m_Ragdoll.isGettingUp)
            {
                m_Rigidbody.detectCollisions = true;
                m_Rigidbody.isKinematic = false;
                m_Capsule.enabled = true;
            }
        };
    }

}
