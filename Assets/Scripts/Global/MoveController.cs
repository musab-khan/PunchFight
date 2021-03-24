using System.Collections;
using System.Collections.Generic;
using MLSpace;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    public Animator playerController;
    public Animator OpponentController;
    public int animationStates = 5;
    public bool startRagdoll = false;

    public Rigidbody[] allPlayerColliders;
    public Rigidbody[] allOpponentColliders;

    public static MoveController Instance;
    private static readonly int AnimationIndex = Animator.StringToHash("AnimationIndex");

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void OpponentMove()
    {
        for (int i = 0; i < allPlayerColliders.Length; i++)
        {
            allPlayerColliders[i].isKinematic = true;
            allPlayerColliders[i].gameObject.GetComponent<CapsuleCollider>().enabled = false;
        }
        int animIndex = RandomGenerator.GenerateInt(0, animationStates);
        OpponentController.SetInteger(AnimationIndex,animIndex);
//        Damage.Instance.PlayerHit();
    }

    public void PlayerMove()
    {
        for (int i = 0; i < allOpponentColliders.Length; i++)
        {
            allOpponentColliders[i].isKinematic = true;
            allOpponentColliders[i].gameObject.GetComponent<CapsuleCollider>().enabled = false;
        }
        int animIndex = RandomGenerator.GenerateInt(0, animationStates);
        playerController.SetInteger(AnimationIndex,animIndex);
//        Damage.Instance.EnemyHit();
    }

    public void ResetRigidBodies()
    {
        for (int i = 0; i < allOpponentColliders.Length; i++)
        {
            allOpponentColliders[i].isKinematic = false;
            allOpponentColliders[i].gameObject.GetComponent<CapsuleCollider>().enabled = true;
        }
        
        for (int i = 0; i < allPlayerColliders.Length; i++)
        {
            allPlayerColliders[i].isKinematic = false;
            allPlayerColliders[i].gameObject.GetComponent<CapsuleCollider>().enabled = true;
        }
    }
}
