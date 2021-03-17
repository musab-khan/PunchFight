
using System;
using UnityEngine;
using MLSpace;


/// <summary>
/// Example of regular physics impact with ragdoll manager.
/// </summary>
public class RigidbodyCollider : MonoBehaviour
{
    private Rigidbody rb;   // reference to rigidbody

    public float hitInterval = 1.0f;        // do mot hit every frame. give it a little buffer.
    private float hitTimer = 0.0f;

    private void OnEnable()
    {
        // get rigidbody component
        rb = GetComponent<Rigidbody>();
        if (!rb) { Debug.LogWarning("Cannot find Rigidbody component."); }
        hitTimer = hitInterval;
    }

    private void Update()
    {
        hitTimer += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hitTimer > hitInterval)
        {
            BodyColliderScript bcs = collision.collider.GetComponent<BodyColliderScript>();
            if (bcs && rb)
            {
                int[] indices = new int[] { bcs.index };

                // starting hit reaction by adding object velocity and mass
                bcs.ParentRagdollManager.startHitReaction(indices, rb.velocity * rb.mass);
            }
            hitTimer = 0.0f;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        print("trigered "+col);
    }
}

