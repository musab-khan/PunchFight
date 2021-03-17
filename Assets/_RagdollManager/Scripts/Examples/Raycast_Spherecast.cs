
using UnityEngine;
using MLSpace;

/// <summary>
/// Example of starting ragdoll manager through raycasting and spherecasting
/// </summary>
public class Raycast_Spherecast : MonoBehaviour
{
    public float hitForce = 16.0f;    // hit force 


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            raycast(ray);
        }
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            spherecast(ray, 0.1f);
        }
    }

    /// <summary>
    /// Shoot ray at character and start ragdoll manager if hit.
    /// </summary>
    /// <param name="ray"></param>
    public void raycast(Ray ray)
    {
        int mask = LayerMask.GetMask ("ColliderLayer", "ColliderInactiveLayer");
        RaycastHit hit;
        if(Physics.Raycast (ray,out hit, Camera.main.farClipPlane , mask))
        {
            BodyColliderScript bcs = hit.collider.GetComponent<BodyColliderScript>();
            if(bcs)
            {
                int[] parts = new int[] { bcs.index };
                bcs.ParentRagdollManager.startHitReaction(parts, ray.direction * hitForce);
            }
        }
    }

    /// <summary>
    /// Shoot sphere at character and start ragdoll manager if hit.
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="radius"></param>
    public void spherecast(Ray ray,float radius)
    {
        int mask = LayerMask.GetMask("ColliderLayer", "ColliderInactiveLayer");
        RaycastHit hit;
        if (Physics.SphereCast (ray, radius, out hit, Camera.main.farClipPlane, mask))
        {
            BodyColliderScript bcs = hit.collider.GetComponent<BodyColliderScript>();
            if (bcs)
            {
                int[] parts = new int[] { bcs.index };
                bcs.ParentRagdollManager.startHitReaction(parts, ray.direction * hitForce);
            }
        }
    }
}
