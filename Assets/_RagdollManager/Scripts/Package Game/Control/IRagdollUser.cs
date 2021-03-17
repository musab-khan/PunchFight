// © 2015 Mario Lelas
using UnityEngine;

namespace MLSpace
{
    /// <summary>
    /// ragdoll interface for ragdoll users
    /// </summary>
    public interface IRagdollUser
    {
        /// <summary>
        /// gets reference to ragdoll manager
        /// </summary>
        RagdollManager ragdollManager { get; }


        /// <summary>
        /// gets bounds of character
        /// </summary>
        Bounds bounds { get; }

        /// <summary>
        /// begin hit reaction
        /// </summary>
        /// <param name="hitParts"></param>
        /// <param name="hitForce"></param>
        void startHitReaction(int[] hitParts, Vector3 hitForce);


        /// <summary>
        /// begin ragdoll
        /// </summary>
        /// <param name="bodyParts"></param>
        /// <param name="bodyPartForce"></param>
        /// <param name="overallForce"></param>
        void startRagdoll(int[] bodyParts, Vector3 bodyPartForce, Vector3 overallForce);

        /// <summary>
        /// gets and sets flag to ignore hits
        /// </summary>
        bool ignoreHit { get; set; }
    } 
}
