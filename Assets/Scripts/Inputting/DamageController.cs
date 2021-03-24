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
            lostPanel.SetActive(true);
            gamePanel.SetActive(false);
            progressPanel.SetActive(false);
        }
    }

    public void EnemyHit()
    {
        enemyProgress.fillAmount -= progressDecrement;
        --enemyHealth;

        if (enemyHealth == 0)
        {
            wonPanel.SetActive(true);
            gamePanel.SetActive(false);
            progressPanel.SetActive(false);
        }
    }

    public void DecideWinner()
    {
        if (playerHealth > enemyHealth)
        {
            wonPanel.SetActive(true);
            gamePanel.SetActive(false);
            progressPanel.SetActive(false);
        }

        else
        {
            lostPanel.SetActive(true);
            gamePanel.SetActive(false);
            progressPanel.SetActive(false);
        }
    }

    public void closePanels()
    {
        wonPanel.SetActive(false);
        gamePanel.SetActive(true);
        progressPanel.SetActive(true);
    }
}
