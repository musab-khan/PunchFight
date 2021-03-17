using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLSpace
{
    public class MineTrigger : MonoBehaviour
    {

        bool armed = false;

        public VoidFunc OnExplode = null;

        bool m_Initialized = false;

        public float triggerRadius { get; private set; }

        // Use this for initialization
        void Start()
        {
            initialize();
        }

        public void initialize()
        {
            if (m_Initialized) return;

            SphereCollider sphereTrigger = GetComponent<SphereCollider>();
            if (!sphereTrigger) sphereTrigger = this.gameObject.AddComponent<SphereCollider>();
            sphereTrigger.isTrigger = true;
            triggerRadius = sphereTrigger.radius;

            m_Initialized = true;
        }

        public void arm()
        {
            if (!m_Initialized) initialize();
            armed = true;
        }

        public void explode()
        {
            if (OnExplode != null)
                OnExplode();
            armed = false;
            countDownStarted = false;
        }
        [HideInInspector ]
        public float countDown = 0.0f;
        [HideInInspector]
        public float countDownLimit = 2.0f;

        [HideInInspector]
        public bool countDownStarted = false;

        private void FixedUpdate()
        {
            if(countDownStarted )
            {
                countDown += Time.deltaTime;
                if (countDown > countDownLimit)
                    explode();
            }
        }

        public void OnTriggerStay(Collider other)
        {
            if (!armed) return;
            if (!m_Initialized) initialize();

            //Debug.Log("Timer started at: " + other.name);
            countDownStarted = true;
        }
    }
}
