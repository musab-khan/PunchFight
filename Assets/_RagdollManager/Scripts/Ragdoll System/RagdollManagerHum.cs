// © 2016 Mario Lelas
using UnityEngine;
using System.Collections.Generic;

namespace MLSpace
{
    /// <summary>
    /// body parts types
    /// </summary>
    public enum BodyParts : int
    {
        Spine = 0,
        Chest,
        Head,
        LeftShoulder,
        RightShoulder,
        LeftElbow,
        RightElbow,
        LeftHip,
        RightHip,
        LeftKnee,
        RightKnee,
        BODY_PART_COUNT,
        None,
    }

    /// <summary>
    /// ragdoll and hit reaction manager on humanoid rigs
    /// </summary>
    public class RagdollManagerHum : RagdollManager
    {


        /// <summary>
        /// use get up animation after ragdoll
        /// </summary>
        [Tooltip("Use get up animation after ragdoll ?"), HideInInspector]
        public bool enableGetUpAnimation = true;

        /// <summary>
        /// name of get up from back animation state clip 
        /// </summary>
        [Tooltip("Name of 'get up from back' animation state."), HideInInspector]
        public string nameOfGetUpFromBackState = "GetUpBack";

        /// <summary>
        /// name of get up from front animation state clip 
        /// </summary>
        [Tooltip("Name of 'get up from front' animation state."), HideInInspector]
        public string nameOfGetUpFromFrontState = "GetUpFront";

        // transform used for getting up orientation
        // forward axis must point towards character front
        protected Transform m_OrientTransform;

        /// <summary>
        /// gets number of bodyparts
        /// </summary>
        public override int bodypartCount
        {
            get { return (int)BodyParts.BODY_PART_COUNT; }
        }

        /// <summary>
        /// initialize class instance
        /// </summary>
        public override void initialize()
        {
            if (m_Initialized) return;

            base.initialize();


            /* 
                NOTE:

                m_OrientTransform should be hips transform with forward vector 
                pointing forwards of character , 
                but I found out that not all models hips bone transform are oriented that way ( ?? ).
                So I am creating new object as m_OrientTransform to be  oriented as character, 
                but positioned on hip transform.

                If your hip bone is oriented so its looking in character forward directioon,
                you can assign hip transform as m_OrientTransform and not create new object.
                Or you can make m_OrientTransform field public and assign orient transform as you wish.

                I made it this way so it would be less setup for users.

            */

            GameObject orientTransformObj = new GameObject("OrientTransform");
            orientTransformObj.transform.position = RagdollBones[(int)BodyParts.Spine].position;
            orientTransformObj.transform.rotation = this.transform.rotation;
            orientTransformObj.transform.SetParent(RagdollBones[(int)BodyParts.Spine]);
            m_OrientTransform = orientTransformObj.transform;

            m_InitialMode = m_Animator.updateMode;
            m_InitialRootMotion = m_Animator.applyRootMotion;
            m_RootTransform = m_BodyParts[(int)BodyParts.Spine].transform ;


            List<int> constraint_indices = new List<int>();
            constraint_indices.Add((int)BodyParts.LeftKnee);
            constraint_indices.Add((int)BodyParts.RightKnee);
            createConstraints(constraint_indices);

            

            m_Initialized = true;

            _disableRagdoll(false);
        }

        /// <summary>
        /// get info on body part based on humaniod enumeration
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public BodyPartInfo getBodyPartInfo(BodyParts part)
        {
#if DEBUG_INFO
            if (!m_Initialized)
            {
                Debug.LogError("component not initialized.");
                return null;
            }
            if (m_BodyParts == null) { Debug.LogError("object cannot be null."); return null; }
#endif
            if (part < BodyParts.BODY_PART_COUNT)
                return m_BodyParts[(int)part];
            else return null;
        }


#region UNITY_METHODS

        // Unity start method
        void Start()
        {
            // initialize all
            initialize();
        }

#if DEBUG_INFO
        void OnDrawGizmos()
        {
            if (m_BodyParts == null) return;
            if (m_BodyParts.Length == 0) return;

            if (!m_Animator)
                m_Animator = GetComponent<Animator>();

            Gizmos.color = Color.white;
            Gizmos.DrawLine(m_BodyParts[(int)BodyParts.Spine].transform.position,
                m_BodyParts[(int)BodyParts.Chest].transform.position);
            Gizmos.DrawLine(m_BodyParts[(int)BodyParts.Chest].transform.position,
                m_BodyParts[(int)BodyParts.Head].transform.position);
            Gizmos.DrawLine(m_BodyParts[(int)BodyParts.Chest].transform.position,
                m_BodyParts[(int)BodyParts.LeftShoulder].transform.position);
            Gizmos.DrawLine(m_BodyParts[(int)BodyParts.Chest].transform.position,
                m_BodyParts[(int)BodyParts.RightShoulder].transform.position);
            Gizmos.DrawLine(m_BodyParts[(int)BodyParts.LeftShoulder].transform.position,
                m_BodyParts[(int)BodyParts.LeftElbow].transform.position);
            Gizmos.DrawLine(m_BodyParts[(int)BodyParts.RightShoulder].transform.position,
                m_BodyParts[(int)BodyParts.RightElbow].transform.position);
            Gizmos.DrawLine(m_BodyParts[(int)BodyParts.Spine].transform.position,
                m_BodyParts[(int)BodyParts.LeftHip].transform.position);
            Gizmos.DrawLine(m_BodyParts[(int)BodyParts.Spine].transform.position,
                m_BodyParts[(int)BodyParts.RightHip].transform.position);
            Gizmos.DrawLine(m_BodyParts[(int)BodyParts.LeftHip].transform.position,
                m_BodyParts[(int)BodyParts.LeftKnee].transform.position);
            Gizmos.DrawLine(m_BodyParts[(int)BodyParts.RightHip].transform.position,
                m_BodyParts[(int)BodyParts.RightKnee].transform.position);

            Transform lhand = m_BodyParts[(int)BodyParts.LeftElbow].transform.GetChild(0);
            Transform rhand = m_BodyParts[(int)BodyParts.RightElbow].transform.GetChild(0);
            Transform lfoot = m_BodyParts[(int)BodyParts.LeftKnee].transform.GetChild(0);
            Transform rfoot = m_BodyParts[(int)BodyParts.RightKnee].transform.GetChild(0);

            Gizmos.DrawLine(m_BodyParts[(int)BodyParts.LeftElbow].transform.position, lhand.position);
            Gizmos.DrawLine(m_BodyParts[(int)BodyParts.RightElbow].transform.position, rhand.position);
            Gizmos.DrawLine(m_BodyParts[(int)BodyParts.LeftKnee].transform.position, lfoot.position);
            Gizmos.DrawLine(m_BodyParts[(int)BodyParts.RightKnee].transform.position, rfoot.position);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(m_Animator.GetBoneTransform(HumanBodyBones.Hips).position,
                m_Animator.GetBoneTransform(HumanBodyBones.Chest).position);
            Gizmos.DrawLine(m_Animator.GetBoneTransform(HumanBodyBones.Chest).position,
                m_Animator.GetBoneTransform(HumanBodyBones.Head).position);
            Gizmos.DrawLine(m_Animator.GetBoneTransform(HumanBodyBones.Chest).position,
                m_Animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).position);
            Gizmos.DrawLine(m_Animator.GetBoneTransform(HumanBodyBones.Chest).position,
                m_Animator.GetBoneTransform(HumanBodyBones.RightUpperArm).position);
            Gizmos.DrawLine(m_Animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).position,
                m_Animator.GetBoneTransform(HumanBodyBones.LeftLowerArm).position);
            Gizmos.DrawLine(m_Animator.GetBoneTransform(HumanBodyBones.LeftLowerArm).position,
                m_Animator.GetBoneTransform(HumanBodyBones.LeftHand).position);
            Gizmos.DrawLine(m_Animator.GetBoneTransform(HumanBodyBones.RightUpperArm).position,
                m_Animator.GetBoneTransform(HumanBodyBones.RightLowerArm).position);
            Gizmos.DrawLine(m_Animator.GetBoneTransform(HumanBodyBones.RightLowerArm).position,
                m_Animator.GetBoneTransform(HumanBodyBones.RightHand).position);
            Gizmos.DrawLine(m_Animator.GetBoneTransform(HumanBodyBones.Hips).position,
                m_Animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position);
            Gizmos.DrawLine(m_Animator.GetBoneTransform(HumanBodyBones.Hips).position,
                m_Animator.GetBoneTransform(HumanBodyBones.RightUpperLeg).position);
            Gizmos.DrawLine(m_Animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position,
                m_Animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position);
            Gizmos.DrawLine(m_Animator.GetBoneTransform(HumanBodyBones.RightUpperLeg).position,
                m_Animator.GetBoneTransform(HumanBodyBones.RightLowerLeg).position);
            Gizmos.DrawLine(m_Animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position,
                m_Animator.GetBoneTransform(HumanBodyBones.LeftFoot).position);
            Gizmos.DrawLine(m_Animator.GetBoneTransform(HumanBodyBones.RightLowerLeg).position,
                m_Animator.GetBoneTransform(HumanBodyBones.RightFoot).position);
        }
#endif

#endregion

        /// <summary>
        /// start humanoid rig transition to mecanim
        /// </summary>
        protected override void _startTransition()
        {
            if (m_state != RagdollState.Ragdoll) return;
#if DEBUG_INFO
            if (!m_Animator) { Debug.LogError("object cannot be null - " + this.name); return; }
            if (m_BodyParts == null) { Debug.LogError("object cannot be null"); return; }
            if (!m_OrientTransform) { Debug.LogError("object cannot be null. - " + this.name); return; }
#endif
            _disableRagdoll(false);
            if (useJoints)
            {
                for (int i = 0; i < m_ConstraintIndices.Count; i++)
                {
                    m_BodyParts[m_ConstraintIndices[i]].constraintJoint.xMotion = ConfigurableJointMotion.Free;
                    m_BodyParts[m_ConstraintIndices[i]].constraintJoint.yMotion = ConfigurableJointMotion.Free;
                    m_BodyParts[m_ConstraintIndices[i]].constraintJoint.zMotion = ConfigurableJointMotion.Free;
                }
            }
            else
            {
                for (int i = 1; i < bodypartCount; i++)
                {
                    BodyPartInfo b = m_BodyParts[i];
                    b.transform.SetParent(b.orig_parent);
                }
            }
            foreach (BodyPartInfo b in m_BodyParts)
            {
                b.transitionRotation = b.transform.rotation;
                b.transitionPosition = b.transform.position;
            }
            m_CurrentBlendTime = 0.0f;
            m_Animator.enabled = true; //enable animation
            m_Animator.updateMode = AnimatorUpdateMode.Normal; // set animator update to normal
            m_Animator.applyRootMotion = true;
            m_state = RagdollState.Blend;
#if SAVE_ANIMATOR_STATES
                        resetAnimatorStates();
#endif
            if (m_GettingUp && !m_HitReacWhileGettingUp)
            {
                m_Animator.applyRootMotion = false; // problems when getting up. must be false
                Vector3 newRootPosition = m_OrientTransform.position;
                // shoot ray to check ground and set new root position on ground
                // comment or delete this if you dont want this feature
                Vector3 raypos = m_OrientTransform.position + Vector3.up * 0.01f;
                Ray ray = new Ray(raypos, Vector3.down);
                // ignore colliders
                int layerMask = ~LayerMask.GetMask(this.activeColLayerName, this.inactiveColLayerName);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 20f, layerMask))
                {
                    newRootPosition.y = hit.point.y;
                    newRootPosition.x = hit.point.x;
                    newRootPosition.z = hit.point.z;
                }
                bool upwards = m_OrientTransform.forward.y > 0.0f;
                if (upwards)
                {
                    if (m_GettingUpEnableInternal && enableGetUpAnimation)
                    {
                        m_Animator.CrossFade(nameOfGetUpFromFrontState, 0.0f, 0, 0.0f);
                        Vector3 _up = -m_OrientTransform.up;
                        _up.y = 0.0f;
                        transform.forward = _up;
                    }
                }
                else
                {
                    if (m_GettingUpEnableInternal && enableGetUpAnimation)
                    {
                        m_Animator.CrossFade(nameOfGetUpFromBackState, 0.0f, 0, 0.0f);
                        Vector3 _up = m_OrientTransform.up;
                        _up.y = 0.0f;
                        transform.forward = _up;
                    }
                }
                transform.position = newRootPosition;
            }
            m_GettingUpEnableInternal = true;
            if (OnStartTransition != null)
                OnStartTransition();
        }

        /// <summary>
        /// Deprecated. Use method with int[] first parameter.
        /// starts ragdoll flag by adding velocity to chosen body part and overall velocity to all parts
        /// </summary>
        /// <param name="part">hit body parts</param>
        /// <param name="velocityHit">force on hit body parts</param>
        /// <param name="velocityOverall">overall force applied on rest of bodyparts</param>
        public void startRagdoll
            (
            BodyParts[] hit_parts = null,
            Vector3? hitForce = null,
            Vector3? overallHitForce = null,
            bool ignoreHitInverval = false
            )
        {
#if DEBUG_INFO
            if (!m_Initialized)
            {
                Debug.LogError("component not initialized.");
                return;
            }
#endif

            if (hit_parts != null)
            {
                m_HitParts = new int[hit_parts.Length];
                for (int i = 0; i < hit_parts.Length; i++)
                    m_HitParts[i] = (int)hit_parts[i];
            }

            m_CurrentEventTime = 0f;

            m_ForceVel = hitForce;
            m_ForceVelocityOveral = overallHitForce;

            m_IgnoreHitInterval = ignoreHitInverval;
            m_FireRagdoll = true;
        }


        /// <summary>
        /// Deprecated. Use method with int[] first parameter.
        /// Set hit reaction flag and hit velocity
        /// </summary>
        /// <param name="hit_parts">hit parts indices</param>
        /// <param name="forceVelocity"></param>
        public void startHitReaction(
            BodyParts[] hit_parts,
            Vector3 forceVelocity,
            bool ignoreHitInterval = false)
        {
#if DEBUG_INFO
            if (!m_Initialized)
            {
                Debug.LogError("component not initialized.");
                return;
            }
#endif
            if (hit_parts != null)
            {
                m_HitParts = new int[hit_parts.Length];
                for (int i = 0; i < hit_parts.Length; i++)
                    m_HitParts[i] = (int)hit_parts[i];
            }
            m_ForceVel = forceVelocity;
            m_IgnoreHitInterval = ignoreHitInterval;
            m_FireHitReaction = true;
        }

        public void DisableRagdoll()
        {
            _disableRagdoll(false);
        }

        public void EnableRagdoll()
        {
            _enableRagdoll(false);
        }
    }
}
