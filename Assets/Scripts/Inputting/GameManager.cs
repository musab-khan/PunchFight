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

    public static GameManager Instance;

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

    public void Reload()
    {
        SceneManager.LoadScene(2);
    }
    
    public void mainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
