using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPoints : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        foreach (Transform child in transform)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(child.position, 20f);
        }
    }
}
