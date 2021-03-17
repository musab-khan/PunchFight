using System;
using System.Collections;
using System.Collections.Generic;
using MLSpace;
using UnityEngine;

public class CollisionDetect : MonoBehaviour
{
    private Rigidbody rb;
    public float forceValue = 10.0f;// reference to rigidbody
    private LayerMask player;

    private float hitInterval = 1.0f;
    private float hitTimer = 0.0f;

    private bool firstHit = false;
    private void OnEnable()
    {
        player = LayerMask.GetMask("Player");
        // get rigidbody component
        rb = GetComponent<Rigidbody>();
        if (!rb) { Debug.LogWarning("Cannot find Rigidbody component."); }
    }

    void Update()
    {
        hitTimer += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (hitTimer >= hitInterval)
        {

//            Debug.Log($"body part{other.collider.name} character {other.collider.transform.root.name}");
            ContactPoint contact = other.contacts[0];
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
        
            BodyColliderScript bcs = other.collider.GetComponent<BodyColliderScript>();
            if (bcs)
            {
                int[] indices = new int[] { bcs.index };   
                // starting hit reaction by adding object velocity and mass
                bcs.ParentRagdollManager.startHitReaction(indices, rotation.eulerAngles  * 0.01f);
            }

            hitTimer = 0.0f;
        }
    }
}
