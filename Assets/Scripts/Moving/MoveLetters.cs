using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveLetters : MonoBehaviour
{
    [SerializeField] private GameObject letters;
    [SerializeField] private RectTransform pointStart;
    [SerializeField] private RectTransform pointEnd;
    [SerializeField] private List<RectTransform> lettersList;
    [SerializeField] private float nextSpawnDelay;
    [SerializeField] private Queue<LetterProperties> activeTiles;
    
    [Space]
    [Header("Spawn Times")]
    [SerializeField] [Range(1f, 3f)] float minDuration;
    [SerializeField] [Range(3f, 5f)] float maxDuration;
    
    [Space]
    [Header("Move Times")]
    [SerializeField] [Range(1f, 2f)] float moveMinTime;
    [SerializeField] [Range(2f, 4f)] float moveMaxTime;

    [Space] 
    [Header("Effects")] 
    [SerializeField] private ParticleSystem popEffect;

    public static MoveLetters Instance;

    void Awake()
    {
        if (!Instance)
            Instance = this;
    }

    public void Init()
    {
        lettersList = new List<RectTransform>();
        string temp = LevelManager.Instance.levels[LevelManager.Instance.curLevel].word;
        
        activeTiles = new Queue<LetterProperties>();
        
        foreach (Transform child in letters.transform)
        {
            for (int i = 0; i < temp.Length; i++)
            {
                if (child.GetComponent<LetterProperties>().Character.Equals(temp[i].ToString()))
                {
                    child.transform.localScale = Vector3.one;
                    lettersList.Add(child.GetComponent<RectTransform>());       
                }
            }
        }
        
        updateDelay();
    }
    
    
    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextSpawnDelay && LevelManager.Instance.isLevelRunning && !LevelManager.Instance.levelEnded)
        {
            MoveLetter();
            updateDelay();
        }
    }
    
    void updateDelay()
    {
        float timeDelay = RandomGenerator.GenerateFloat(minDuration, maxDuration);
        nextSpawnDelay = Time.time + timeDelay;
    }

    void MoveLetter()
    {
        var randomLetter = RandomGenerator.GenerateInt(0, lettersList.Count);
        RectTransform rect = lettersList[randomLetter];
        rect.gameObject.SetActive(true);
        activeTiles.Enqueue(rect.gameObject.GetComponent<LetterProperties>());

        float duration =  RandomGenerator.GenerateFloat(moveMinTime, moveMaxTime);

        LeanTween.moveX(rect, pointEnd.position.x - 150f, duration).setOnComplete(() =>
        {
            if (rect.gameObject.activeSelf)
            {
                activeTiles.Dequeue();
                lettersList[randomLetter].gameObject.SetActive(false);
                rect.transform.position = pointStart.position;
            }
        });
    }

    public void onButtonClick(GameObject obj)
    {
        if (activeTiles.Count == 0)
            return;
        
        if (!obj.GetComponent<LetterProperties>())
        {
            Debug.Log("wrongPress");
            LeanTween.scale(obj.transform.GetChild(1).GetComponent<RectTransform>(), Vector3.one, 0.5f).setEase(LeanTweenType.easeSpring).setLoopPingPong(1);
            MoveController.Instance.OpponentMove();
            Damage.Instance.PlayerHit();
        }

        else
        {
            if (obj.GetComponent<LetterProperties>().Character.Equals(activeTiles.Peek().Character))
            {
                Debug.Log("correctPress");
                MoveController.Instance.PlayerMove();
//              activeTiles.Peek().gameObject.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                LeanTween.cancel(activeTiles.Peek().gameObject);
                LeanTween.scale(activeTiles.Peek().gameObject, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInBack).setOnComplete(() =>
                {
                    activeTiles.Peek().gameObject.SetActive(false);
                    activeTiles.Dequeue();
                });
                popEffect.transform.position = activeTiles.Peek().gameObject.transform.position;
                popEffect.Play();
                lettersList.Remove(activeTiles.Peek().gameObject.GetComponent<RectTransform>());
                
                MoveController.Instance.PlayerMove();
                Damage.Instance.EnemyHit();
                
                if (lettersList.Count == 0)
                {
                    LevelManager.Instance.levelEnded = true;
                    MoveController.Instance.startRagdoll = true;
                    Damage.Instance.LevelFinished();
//                    Damage.Instance.DecideWinner();
                }
            }
            else
            {
                Debug.Log("wrongPress");
                LeanTween.scale(obj.transform.GetChild(1).GetComponent<RectTransform>(), Vector3.one, 0.5f).setEase(LeanTweenType.easeSpring).setLoopPingPong(1);
                MoveController.Instance.OpponentMove();
                Damage.Instance.PlayerHit();
            }
        }
    }
    
    [ContextMenu("SetCharacters")]
    void SetAlphabetCharacter()
    {
        foreach (Transform child in letters.transform)
        {
            LetterProperties letter = child.gameObject.AddComponent<LetterProperties>();
            letter.Character = child.name;
        }
    }
}
