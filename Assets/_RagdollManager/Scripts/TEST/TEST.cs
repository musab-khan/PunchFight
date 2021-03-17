using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLSpace
{
    public class TEST : MonoBehaviour
    {
        private RagdollManager m_Ragdoll;


        // Use this for initialization
        void Start()
        {
            m_Ragdoll = GetComponent<RagdollManager>();

            //m_Ragdoll.OnHit = () =>
            //{
            //    //m_Rigidbody.velocity = Vector3.zero;
            //    //m_Rigidbody.detectCollisions = false;
            //    //m_Rigidbody.isKinematic = true;
            //    //m_Capsule.enabled = false;
            //    Debug.Log("Ragdoll hit!");
            //};
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                m_Ragdoll.blendToMecanim();
            }
        }
    } 
}
