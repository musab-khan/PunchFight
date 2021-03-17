// © 2015 Mario Lelas

using UnityEngine;

namespace MLSpace
{
    /// <summary>
    ///  Helper class derived from ColliderScript that hold additional information about body part
    /// </summary>
    public class BodyColliderScript : ColliderScript
    {
        /// <summary>
        /// you can apply additional damage if critial
        /// </summary>
        public bool critical = false;

        /// <summary>
        /// collider body part
        /// </summary>
        public BodyParts bodyPart = BodyParts.None ;

        /// <summary>
        /// index of collider
        /// </summary>
        public int index = -1;

        /// <summary>
        /// reference to parents ragdollmanager script
        /// </summary>
        [SerializeField, HideInInspector]
        private RagdollManager m_ParentRagdollManager;  

        /// <summary>
        /// gets and sets reference to parents ragdoll manager script
        /// </summary>
        public RagdollManager ParentRagdollManager
        {
            get { return m_ParentRagdollManager; }
            set { m_ParentRagdollManager = value; }
        }
    } 
}
