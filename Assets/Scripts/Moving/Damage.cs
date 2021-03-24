using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
   public static Damage Instance;
   
   public  int playerHealth = 0;
   public int enemyHealth = 0;

   private float progressDecrement;
   
   [SerializeField] private Image playerProgress;
   [SerializeField] private Image enemyProgress;

   private void Awake()
   {
      if (Instance == null)
         Instance = this;
   }

   public void initDamage()
   {
      playerHealth = enemyHealth = LevelManager.Instance.levels[LevelManager.Instance.curLevel].word.Length;
      progressDecrement = 1f / LevelManager.Instance.levels[LevelManager.Instance.curLevel].word.Length;
   }

   public void PlayerHit()
   {
      playerProgress.fillAmount -= progressDecrement;
      --playerHealth;

      if (playerHealth <= 0)
      {
         LevelManager.Instance.levelEnded = true;
         MoveController.Instance.startRagdoll = true;
         LevelFinished();
//         LevelManager.Instance.LevelFailed();
      }
   }

   public void EnemyHit()
   {
      enemyProgress.fillAmount -= progressDecrement;
      --enemyHealth;

      if (enemyHealth <= 0)
      {
         LevelManager.Instance.levelEnded = true;
         MoveController.Instance.startRagdoll = true;
         LevelFinished();
//         LevelManager.Instance.LevelWon();
      }
   }

   public void DecideWinner()
   {
      if (playerHealth >= enemyHealth)
      {
         LevelManager.Instance.LevelWon();
      }

      else
      {
         LevelManager.Instance.LevelFailed();
      }
   }

   public void LevelFinished()
   {
      StartCoroutine(LevelEnd());
//      Time.timeScale = 0.3f;
   }

   IEnumerator LevelEnd()
   {
      yield return new WaitForSeconds(5.0f);
      DecideWinner();
   }
}
