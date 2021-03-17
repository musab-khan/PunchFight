using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
   public static Damage Instance;
   
   public  int playerDamage = 0;
   public int enemyDamage = 0;

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
      playerDamage = enemyDamage = LevelManager.Instance.levels[LevelManager.Instance.curLevel].word.Length;
      progressDecrement = 1f / LevelManager.Instance.levels[LevelManager.Instance.curLevel].word.Length;
   }

   public void PlayerHit()
   {
      playerProgress.fillAmount -= progressDecrement;
      --playerDamage;

      if (playerDamage <= 0)
      {
         LevelManager.Instance.levelEnded = true;
         LevelManager.Instance.LevelFailed();
      }
   }

   public void EnemyHit()
   {
      enemyProgress.fillAmount -= progressDecrement;
      --enemyDamage;
   }
}
