using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageController : MonoBehaviour
{
    public static DamageController Instance;
   
    public  int playerHealth = 0;
    public int enemyHealth = 0;

    private float progressDecrement;
   
    [SerializeField] private Image playerProgress;
    [SerializeField] private Image enemyProgress;

    public GameObject wonPanel;
    public GameObject lostPanel;
    public GameObject gamePanel;
    public GameObject progressPanel;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void initDamage()
    {
        playerProgress.fillAmount = 1f;
        enemyProgress.fillAmount = 1f;
        
        playerHealth = enemyHealth = GameManager.Instance.Levels[GameManager.Instance.curLevel].GetComponent<LevelWords>().allLevelWords.Length;
        progressDecrement = 1f / playerHealth;
    }

    public void PlayerHit()
    {
        playerProgress.fillAmount -= progressDecrement;
        --playerHealth;

        if (playerHealth == 0)
        {
            GameManager.Instance.canInput = false;
            MoveController.Instance.startRagdoll = true;
            LevelFinished();
        }
        MoveController.Instance.OpponentMove();
    }

    public void EnemyHit()
    {
        enemyProgress.fillAmount -= progressDecrement;
        --enemyHealth;

        if (enemyHealth == 0)
        {
            GameManager.Instance.canInput = false;
            MoveController.Instance.startRagdoll = true;
            LevelFinished();
        }
        MoveController.Instance.PlayerMove();
    }

    public void DecideWinner()
    {
        if (playerHealth > enemyHealth)
        {
            GameManager.Instance.LevelWon();
        }

        else
        {
            GameManager.Instance.LevelFailed();
        }
    }

    public void LastMove()
    {
        StartCoroutine(LastMoveWait());
    }

    public void closePanels()
    {
        wonPanel.SetActive(false);
        gamePanel.SetActive(true);
        progressPanel.SetActive(true);
    }
    
    public void LevelFinished()
    {
        StartCoroutine(LevelEnd());
        Time.timeScale = 0.3f;
    }

    IEnumerator LevelEnd()
    {
        yield return new WaitForSeconds(2.0f);
        DecideWinner();
    }

    IEnumerator LastMoveWait()
    {
        yield return new WaitForSeconds(3.0f);
        
        if (playerHealth > enemyHealth)
        {
            MoveController.Instance.startRagdoll = true;
            MoveController.Instance.PlayerMove();
            LevelFinished();
        }

        else
        {
            MoveController.Instance.startRagdoll = true;
            MoveController.Instance.OpponentMove();
            LevelFinished();
        }
    }
}
