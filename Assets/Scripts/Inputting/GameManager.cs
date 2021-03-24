using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject[] Levels;
    public int curLevel = 0;
    public WordGenerator generator;
    
    public GameObject wonPanel;
    public GameObject lostPanel;
    public GameObject GamePlayPanel;
    public GameObject ProgressPanel;

    public static GameManager Instance;

    public bool canInput = true;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
    }

    private void Start()
    {
        generator.GenerateTiles();
        DamageController.Instance.initDamage();
    }

    public void NextLevel()
    {
        ++curLevel;
        if (curLevel < Levels.Length)
        {
            generator.DoNextLevel();
            DamageController.Instance.initDamage();
            DamageController.Instance.closePanels();
        }

        else
        {
            Reload();
        }
    }
    
    public void LevelWon()
    {
        GamePlayPanel.SetActive(false);
        ProgressPanel.SetActive(false);
        wonPanel.SetActive(true);
        LeanTween.scale(wonPanel, Vector3.one, 0.5f).setEase(LeanTweenType.easeSpring);
    }

    public void LevelFailed()
    {
        GamePlayPanel.SetActive(false);
        ProgressPanel.SetActive(false);
        lostPanel.SetActive(true);
        LeanTween.scale(lostPanel, Vector3.one, 0.5f).setEase(LeanTweenType.easeSpring);
    }

    public void Reload()
    {
        SceneManager.LoadScene(2);
    }
    
    public void mainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
