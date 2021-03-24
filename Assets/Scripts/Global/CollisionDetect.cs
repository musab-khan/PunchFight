using System;
using System.Collections;
using System.Collections.Generic;
using MLSpace;
using UnityEngine;

public class CollisionDetect : MonoBehaviour
{
    private Rigidbody rb;
    private float forceValue = 0.03f;

    private float hitInterval = 1.5f;
    private float hitTimer = 0.0f;

    private bool firstHit = false;
    public GameObject hitEffect;
    
    private CameraShake cameraShaker;
    private void OnEnable()
    {
        // get rigidbody component
        rb = GetComponent<Rigidbody>();
        if (!rb) { Debug.LogWarning("Cannot find Rigidbody component."); }

        cameraShaker = Camera.main.GetComponent<CameraShake>();
    }
    
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log($"hitter{gameObject.transform.root.name} body part{ other.collider.name } character { other.collider.transform.root.name }");
        hitEffect.SetActive(false);
        
        BodyColliderScript bcs = other.collider.GetComponent<BodyColliderScript>();
        
        if (bcs)
        {
            ContactPoint contact = other.contacts[0];
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
            int[] indices = new int[] { bcs.index };
            hitEffect.transform.position = contact.point;
            hitEffect.SetActive(true);
            cameraShaker.shakeDuration = 1f;
            
            // starting hit reaction by adding the hit reaction and a small force value

            if (MoveController.Instance.startRagdoll)
            {
                Debug.Log("Ragdoll Started");
                bcs.ParentRagdollManager.startRagdoll(indices);
            }
            else
            {
                bcs.ParentRagdollManager.startHitReaction(indices, rotation.eulerAngles  * forceValue);
            }
        }
    }
}
