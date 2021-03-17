using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLSpace
{
    public class Mine : ProjectileBase
    {

        public float timer = 2.0f;
        public AudioClip[] beepSounds;
        public GameObject indicator;

        private AudioSource m_ASource;
        private bool isStuck = false;



        Material mat;
        MineTrigger mineTrigger;
        ExplosionManager exMan;
        Rigidbody rb;
        Transform parentObj = null;

        public override string ToString()
        {
            return "Mine";
        }

        private void Start()
        {
            MeshRenderer mr = indicator.GetComponent<MeshRenderer>();
            if(mr)mat = mr.material;
            mat.EnableKeyword("_EMISSION");

            rb = GetComponent<Rigidbody>();

            mineTrigger = GetComponentInChildren<MineTrigger>();
            mineTrigger.gameObject.SetActive(false);
            mineTrigger.countDownLimit = timer;

            exMan = FindObjectOfType<ExplosionManager>();
            if(!exMan) { Debug.LogError("Cannot find 'ExplosionManager' on scene.");return; }

            mineTrigger.initialize ();

            mineTrigger.OnExplode = () =>
            {
                exMan.explodeAt(transform.position, mineTrigger.triggerRadius, 1.0f, hitStrength );
                Destroy(this.gameObject);
            };

        }

        public override void setup()
        {
        }

        float beepTime = 0.25f;
        float beepTimer = 0.0f;

        Vector3 posOffset = Vector3.zero;
        Quaternion rotOffset = Quaternion.identity;
        private void FixedUpdate()
        {
            if(parentObj)
            {

                transform.position = parentObj.TransformPoint(posOffset);
                transform.rotation = parentObj.rotation * rotOffset;
            }

            if(mineTrigger .countDownStarted)
            {
                float _sin = Mathf.Sin(Time.time * 10.0f * mineTrigger.countDown);
                Color col = Color.yellow * _sin;
                mat.color = col;
                mat.SetColor("_EmissionColor", col);

                beepTimer += Time.deltaTime ;
                if(beepTimer > beepTime)
                {
                    GameUtils.PlayRandomClipAtPosition(beepSounds, transform.position);
                    beepTimer = 0.0f;
                }

            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (isStuck) return;

            Vector3 n = collision.contacts[0].normal;
            Vector3 p = collision.contacts[0].point;

            transform.position = p + n * 0.01f;
            transform.up = n;
            rb.isKinematic = true;
            rb.detectCollisions = false;

            parentObj = collision.contacts[0].otherCollider.transform;
            posOffset = parentObj.InverseTransformPoint(transform.position);
            rotOffset = Quaternion.Inverse(parentObj.rotation) * transform.rotation;
            

            mineTrigger.gameObject.SetActive(true);
            mineTrigger.arm();

            isStuck = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (isStuck) return;

            Vector3 n = collision.contacts[0].normal;
            Vector3 p = collision.contacts[0].point;

            transform.position = p + n * 0.01f;
            transform.up = n;
            rb.isKinematic = true;
            rb.detectCollisions = false;

            parentObj = collision.contacts[0].otherCollider.transform;
            posOffset = parentObj.InverseTransformPoint(transform.position);
            rotOffset = Quaternion.Inverse(parentObj.rotation) * transform.rotation;


            mineTrigger.gameObject.SetActive(true);
            mineTrigger.arm();

            isStuck = true;
        }
    }

}