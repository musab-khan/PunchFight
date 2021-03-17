
using UnityEngine;

namespace MLSpace
{
    public class PlayerShootBodyScript : MonoBehaviour
    {
        public RigidBodyBullet bullet_prefab;
        public Transform shoot_transform;
        public GameObject owner;

        public float hitForce = 16.0f;


        // Use this for initialization
        void Start()
        {
            if (!bullet_prefab) Debug.LogError("bullet_prefab not assigned");
            if (!shoot_transform) Debug.LogError("shoot_transform not assigned");
            if (!owner) Debug.LogError("owner not assigned");
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (Input.GetButtonDown("Fire1"))
                shoot();
        }

        void shoot()
        {
            RigidBodyBullet newObj = (RigidBodyBullet)Instantiate(bullet_prefab, shoot_transform.position, shoot_transform.rotation);
            Rigidbody rb = newObj.RigidBody ;
            if (!rb) { Debug.LogError("Bullet object does not have Rigidbody component.");return; }
            rb.position = shoot_transform.position;
            rb.rotation = shoot_transform.rotation;
            rb.velocity = shoot_transform.forward * hitForce;
        }
    } 
}
