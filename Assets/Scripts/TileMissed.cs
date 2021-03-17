using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMissed : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Tiles"))
        {
            MoveController.Instance.OpponentMove();
            Damage.Instance.PlayerHit();
        }
    }
}
