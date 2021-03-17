// © 2015 Mario Lelas
using UnityEngine;

/*
    Examples of  RagdollManager usage.
    Add force on body part
    Connect with fixed joint
    Connect with configurable joint 
*/


namespace MLSpace
{
    public class TestWebScene2 : MonoBehaviour
    {
        public Transform parentToBe;


        private RagdollManagerHum m_Ragdoll;
        private UnityEngine.AI.NavMeshAgent m_Agent;
        private ThirdPersonCharacterUnity character; // the character we are controlling

        // Use this for initialization
        void Start()
        {
            m_Ragdoll = GetComponent<RagdollManagerHum>();
            m_Agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            Animator anim = GetComponent<Animator>();
            character = GetComponent<ThirdPersonCharacterUnity>();
            //if (!character) Debug.LogWarning ("No character");

            m_Ragdoll.OnHit = () =>
            {
                anim.applyRootMotion = false;
                if (character) character.simulateRootMotion = false;
                if (m_Agent) m_Agent.enabled = false;
            };
            m_Ragdoll.LastEvent = () =>
            {
                if (character) character.simulateRootMotion = true;

                anim.applyRootMotion = true;
                if (m_Agent) m_Agent.enabled = true;
            };
        }

        //// Update is called once per frame
        //void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.F))
        //    {
        //        addBodypartForce();
        //    }
        //    if(Input.GetKeyDown (KeyCode .G))
        //    {
        //        addFixedJoint();
        //    }
        //    if(Input.GetKeyDown (KeyCode .H))
        //    {
        //        addConfigurableJoint();
        //    }
        //}

        private void addBodypartForce()
        {
            m_Ragdoll.startRagdoll();
            Vector3 v = new Vector3(0.0f, m_Ragdoll.hitReactionTimeModifier, 0.0f);
            RagdollManager.BodyPartInfo b = m_Ragdoll.getBodyPartInfo((int)BodyParts.LeftElbow);
            b.extraForce = v;


            m_Ragdoll.ragdollEventTime = 3.0f;
            m_Ragdoll.OnTimeEnd = () =>
            {
                m_Ragdoll.startRagdoll(null, Vector3.zero, Vector3.zero, true);

                b = m_Ragdoll.getBodyPartInfo((int)BodyParts.LeftElbow);
                b.extraForce = Vector3.zero;

                m_Ragdoll.ragdollEventTime = 5.0f;
                m_Ragdoll.OnTimeEnd = () =>
                {
                    m_Ragdoll.blendToMecanim();
                };
            };
        }

        private void addFixedJoint()
        {
            m_Ragdoll.startRagdoll();
            RagdollManager.BodyPartInfo b = m_Ragdoll.getBodyPartInfo((int)BodyParts.RightKnee );

            // create and add fixed joint on right knee bodypart
            // when ragdoll timer reaches event time destroy it.

            FixedJoint fj = b.rigidBody.gameObject.AddComponent<FixedJoint>();
            fj.connectedBody = parentToBe.GetComponent<Rigidbody>();
            fj.connectedAnchor = Vector3.zero;

            m_Ragdoll.ragdollEventTime = 6.0f;
            m_Ragdoll.OnTimeEnd = () =>
            {
                m_Ragdoll.startRagdoll(null, Vector3.zero, Vector3.zero, true);

                b = m_Ragdoll.getBodyPartInfo((int)BodyParts.RightKnee);
                Destroy(b.rigidBody .gameObject .GetComponent <FixedJoint >());

                m_Ragdoll.ragdollEventTime = 5.0f;
                m_Ragdoll.OnTimeEnd = () =>
                {
                    m_Ragdoll.blendToMecanim();
                };
            };
        }

        private void addConfigurableJoint()
        {
            m_Ragdoll.startRagdoll();

            // create and add configurable joint on right knee bodypart
            // when ragdoll timer reaches event time destroy it.

            RagdollManager.BodyPartInfo b = m_Ragdoll.getBodyPartInfo((int)BodyParts.RightKnee);

            ConfigurableJoint cj = b.rigidBody.gameObject.AddComponent<ConfigurableJoint>();
            cj.connectedBody = parentToBe.GetComponent<Rigidbody>();
            cj.connectedAnchor = parentToBe.position;
            SoftJointLimit sjl = new SoftJointLimit();
            sjl.limit = 0.05f;
            cj.linearLimit = sjl;
            cj.xMotion = ConfigurableJointMotion.Limited;
            cj.yMotion = ConfigurableJointMotion.Limited;
            cj.zMotion = ConfigurableJointMotion.Limited;

            m_Ragdoll.ragdollEventTime = 16.0f;
            m_Ragdoll.OnTimeEnd = () =>
            {
                m_Ragdoll.startRagdoll(null, Vector3.zero, Vector3.zero, true);

                b = m_Ragdoll.getBodyPartInfo((int)BodyParts.RightKnee);
                Destroy(b.rigidBody.gameObject.GetComponent<FixedJoint>());

                m_Ragdoll.ragdollEventTime = 5.0f;
                m_Ragdoll.OnTimeEnd = () =>
                {
                    m_Ragdoll.blendToMecanim();
                };
            };
        }
    }
}