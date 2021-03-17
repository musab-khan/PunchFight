using UnityEngine;
using MLSpace;


/// <summary>
/// Shoots ray to hit an object with ragdoll manager on it.
/// </summary>
public class RayShootRM : MonoBehaviour
{
    public float hitForce = 64.0f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            doRagdoll();
        }
        if (Input.GetMouseButtonDown(1))
        {
            doHitReaction();
        }
    }

    private void doHitReaction()
    {
        Ray currentRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        int mask = LayerMask.GetMask("ColliderLayer", "ColliderInactiveLayer");
        RaycastHit rhit;
        if (Physics.Raycast(currentRay, out rhit, 120.0f, mask))
        {
            BodyColliderScript bcs = rhit.collider.GetComponent<BodyColliderScript>();
            int[] parts = new int[] { bcs.index };
            bcs.ParentRagdollManager.startHitReaction(parts, currentRay.direction * hitForce);
        }
    }

    private void doRagdoll()
    {
        Ray currentRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        int mask = LayerMask.GetMask("ColliderLayer", "ColliderInactiveLayer");
        RaycastHit rhit;
        if (Physics.Raycast(currentRay, out rhit, 120.0f, mask))
        {
            BodyColliderScript bcs = rhit.collider.GetComponent<BodyColliderScript>();
            int[] parts = new int[] { bcs.index };
            bcs.ParentRagdollManager.startRagdoll(parts, currentRay.direction * hitForce);
        }
    }
}
