using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordGenerator : MonoBehaviour
{
    private string wordToGenerate;
    public GameObject tilePrefab;
    public GameObject playerPanel;
    public GameObject opponentPanel;
    public GameObject gamePlayPanel;
    public InputField playerInput;

    public int maxTiles;
    [SerializeField] private List<GameObject> playerTiles;
    [SerializeField] private List<GameObject> opponentTiles;

    Queue<GameObject> tileQ = new Queue<GameObject>();
    
    public float xOffset = 55f;
    private float tileWidth;
    private int wordCount = 0;
    private LevelWord[] words;

    private TouchScreenKeyboard keyboard;

    void Awake()
    {
        PreparePool();
        words = GameManager.Instance.Levels[GameManager.Instance.curLevel].GetComponent<LevelWords>().allLevelWords;
//        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
        playerInput.ActivateInputField();
    }

    void Update()
    {
        if (!TouchScreenKeyboard.visible)
        {
            if (Input.touchCount > 0)
            {
//                TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
                playerInput.ActivateInputField();
            }
        }
    }

    void PreparePool()
    {
        playerTiles = new List<GameObject>();
        opponentTiles = new List<GameObject>();

        tileWidth = tilePrefab.GetComponent<RectTransform>().rect.width;
        
        GameObject tile;
        for (int i = 0; i < maxTiles; i++)
        {
            tile = Instantiate(tilePrefab, gamePlayPanel.transform);
            tile.transform.localScale = Vector3.one;
            tile.SetActive(false);
            tileQ.Enqueue(tile);
        }
    }

    public void DoNextLevel()
    {
        words = GameManager.Instance.Levels[GameManager.Instance.curLevel].GetComponent<LevelWords>().allLevelWords;
        GenerateTiles();
    }
    public void GenerateTiles()
    {
        if (wordCount < words.Length)
        {
            wordToGenerate = words[wordCount].word;
            playerInput.characterLimit = wordToGenerate.Length;
            
            playerTiles.Clear();
            opponentTiles.Clear();
        
            foreach (var character in wordToGenerate)
            {
                if (tileQ.Count > 0)
                {
                    GameObject tile = tileQ.Dequeue();
                    tile.transform.SetParent(playerPanel.transform);
                    tile.transform.localScale = Vector3.one;
                    playerTiles.Add(tile);
                }
            }
        
            foreach (var character in wordToGenerate)
            {
                if (tileQ.Count > 0)
                {
                    GameObject tile = tileQ.Dequeue();
                    tile.transform.SetParent(opponentPanel.transform);
                    tile.transform.localScale = Vector3.one;
                    opponentTiles.Add(tile);
                }
            }
        
            PlaceTiles();
        }
        
        else
        {
            GameManager.Instance.canInput = false;
            DamageController.Instance.LastMove();
        }
    }

    void PlaceTiles()
    {
        for (int i = 0; i < playerTiles.Count; i++)
        {
            string letter = wordToGenerate[i].ToString();
            playerTiles[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(tileWidth / 2 + (xOffset * i), 0);
            playerTiles[i].transform.GetChild(0).GetComponent<Text>().text = letter;
            playerTiles[i].GetComponent<LetterProperties>().Character = letter;
            playerTiles[i].SetActive(true);
        }
        
        for (int i = 0; i < opponentTiles.Count; i++)
        {
            string letter = wordToGenerate[i].ToString();
            opponentTiles[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(tileWidth / 2 + (xOffset * i), 0);
            opponentTiles[i].transform.GetChild(0).GetComponent<Text>().text = letter;
            opponentTiles[i].GetComponent<LetterProperties>().Character = letter;
            opponentTiles[i].SetActive(true);
        }

        StartCoroutine(nameof(OpponentTimer));
    }

    public void InputWord()
    {
        if (GameManager.Instance.canInput)
        {
            for (int i = 0; i < playerInput.text.Length; i++)
            {
                if (playerTiles[i].GetComponent<LetterProperties>().Character.Equals(playerInput.text[i].ToString().ToUpper()))
                {
                    playerTiles[i].GetComponent<Image>().color = Color.green;
                }

                else
                {
                    playerTiles[i].GetComponent<Image>().color = Color.red;
                }
            }

            if (playerInput.text.Length == wordToGenerate.Length)
            {
                SubmitInput();
            }
        }
        
    }

    public void SubmitInput()
    {
        if (GameManager.Instance.canInput)
        {
            if (playerInput.text.ToUpper().Equals(wordToGenerate))
            {
                DamageController.Instance.EnemyHit();
                StopAllCoroutines();
                nextWord();
            }

            else
            {
                DamageController.Instance.PlayerHit();
                StopAllCoroutines();
                nextWord();
            }

            playerInput.text = "";
        }
    }

    void nextWord()
    {
        for (int i = 0; i < wordToGenerate.Length; i++)
        {
            playerTiles[i].GetComponent<Image>().color = Color.white;
            opponentTiles[i].GetComponent<Image>().color = Color.white;
            
            playerTiles[i].SetActive(false);
            opponentTiles[i].SetActive(false);
            
            tileQ.Enqueue(playerTiles[i]);
            tileQ.Enqueue(opponentTiles[i]);
        }

        ++wordCount;
        GenerateTiles();
    }

    IEnumerator OpponentTimer()
    {
        for (int i = 0; i < wordToGenerate.Length; i++)
        {
            yield return new WaitForSeconds(2f);
            opponentTiles[i].GetComponent<Image>().color = Color.green;
        }
        
        MoveController.Instance.OpponentMove();
        DamageController.Instance.PlayerHit();
        nextWord();
    }
}