// © 2015 Mario Lelas
using UnityEngine;
using System.Collections.Generic;

namespace MLSpace
{
    public class RagdollManagerGen : RagdollManager
    {
        /// <summary>
        /// physics material to apply to all bodyparts
        /// </summary>
        public PhysicMaterial physicsMaterial = null;

        /// <summary>
        /// gets number of bodyparts
        /// </summary>
        public override int bodypartCount
        {
            get
            {
                if (m_BodyParts == null) return 0;
                return m_BodyParts.Length;
            }
        }

        // Use this for initialization
        void Start()
        {
            initialize();
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

            


            List<int> constraints = new List<int>();
            for (int i = 0; i < m_BodyParts.Length; i++)
            {
                if (m_BodyParts[i].bodyPart == BodyParts.LeftKnee ||
                    m_BodyParts[i].bodyPart == BodyParts.RightKnee)
                {
                    constraints.Add(m_BodyParts[i].index);
                }
            }
            
            createConstraints(constraints);
            _disableRagdoll(false);

            m_Initialized = true;
        }
    } 


}
