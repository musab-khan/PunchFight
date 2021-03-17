using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager: MonoBehaviour
{
    public LevelWord[] levels;
    public int curLevel = 0;
    public static LevelManager Instance;

    public bool isLevelRunning = false;
    public bool levelEnded = false;

    public GameObject wonPanel;
    public GameObject lostPanel;

    void Awake()
    {
        if (!Instance)
            Instance = this;
        
    }

    void Start()
    {
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        isLevelRunning = true;
        GridManager.Instance.StartLevel();
        MoveLetters.Instance.Init();
        Damage.Instance.initDamage();
    }

    public void LevelWon()
    {
        wonPanel.SetActive(true);
        LeanTween.scale(wonPanel, Vector3.one, 0.5f).setEase(LeanTweenType.easeSpring);
    }

    public void LevelFailed()
    {
        lostPanel.SetActive(true);
        LeanTween.scale(lostPanel, Vector3.one, 0.5f).setEase(LeanTweenType.easeSpring);
    }
    
}
