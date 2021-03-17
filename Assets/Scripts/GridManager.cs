using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    private int rows = 2;
    [SerializeField]
    private int columns = 4;
    [SerializeField]
    private float tileSize = 1;
    [SerializeField] 
    private GameObject keyPrefab;
    
    [SerializeField]
    private float offsetX =  70f;
    
    [SerializeField]
    private float offsetY =  30f;
    
    [SerializeField] 
    private List<LetterProperties> letters;

    [SerializeField] 
    private List<GameObject> spawnedButtons;

    [SerializeField]
    private List<int> randomPositions;

    public static GridManager Instance;
    private string word;
    private int min = 0;
    private int max = 25;
    private int totalTiles;
    private int charIndex = 0;
    private int totalCount = 0;

    void Awake()
    {
        if (!Instance)
            Instance = this;
    }

    public void StartLevel()
    {
        word = LevelManager.Instance.levels[LevelManager.Instance.curLevel].word;
        spawnedButtons = new List<GameObject>();
        randomPositions = new List<int>();
        
        totalTiles = rows * columns;
        
        for (int i = 0; i < word.Length; i++)
        {
            int temp = RandomGenerator.GenerateInt(0, totalTiles);
            while (randomPositions.Contains(temp))
            {
                temp = RandomGenerator.GenerateInt(0, totalTiles);
            }
            randomPositions.Add(temp);
        }
        
        for (int i = 0; i < word.Length; i++)
        {
            for (int j = 0; j < letters.Count; j++)
            {
                if (letters[j].Character.Equals(word[i].ToString()))
                {
                    letters.Remove(letters[j]);
                    --max;
                    continue;
                }
            }
        }
        
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                GameObject tile = Instantiate(keyPrefab, transform);
                
                float posX = col * tileSize;
                float posY = row * -tileSize;

                tile.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX + offsetX, posY - offsetY);
                
                if (randomPositions.Contains(totalCount))
                {
                    tile.transform.GetChild(0).GetComponent<Text>().text = word[charIndex].ToString();
                    tile.AddComponent<LetterProperties>().Character = word[charIndex].ToString();
                    charIndex++;
                    spawnedButtons.Add(tile);
                }

                else
                {
                    int temp = RandomGenerator.GenerateInt(min, max--);
                    tile.transform.GetChild(0).GetComponent<Text>().text = letters[temp].Character;
                    letters.Remove(letters[temp]);
                    spawnedButtons.Add(tile);
                }
                ++totalCount;
            }
        }
    }
    
}
