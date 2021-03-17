using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordGenerator : MonoBehaviour
{
    public string wordToGenerate;
    public GameObject tilePrefab;
    public GameObject playerPanel;
    public GameObject opponentPanel;
    private List<GameObject> allTiles;
    private float xOffset = 55f;
    private float initialXPos;
    private float initialYPos;
private int correctInput = 0;

    private void Start()
    {
        allTiles = new List<GameObject>();

        initialXPos = tilePrefab.GetComponent<RectTransform>().anchoredPosition.x;
        initialYPos = tilePrefab.GetComponent<RectTransform>().anchoredPosition.y;
        
        foreach (var character in wordToGenerate)
        {
            GameObject temp = Instantiate(tilePrefab, playerPanel.transform);
            temp.transform.localScale = Vector3.one;
            temp.SetActive(false);
            allTiles.Add(temp);
        }

        foreach (var character in wordToGenerate)
        {
            GameObject temp = Instantiate(tilePrefab, opponentPanel.transform);
            temp.transform.localScale = Vector3.one;
            temp.SetActive(false);
            8/.Add(temp);
        }
        
        PlaceTiles();
    }

    void PlaceTiles()
    {
        for (int i = 0; i < allTiles.Count; i++)
        {
            string letter = wordToGenerate[i].ToString();
            allTiles[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(initialXPos + (xOffset * i), initialYPos);
            allTiles[i].transform.GetChild(0).GetComponent<Text>().text = letter;
            allTiles[i].AddComponent<LetterProperties>().Character = letter;
            allTiles[i].SetActive(true);
        }
    }

    private Image backGround;
    public void LetterClick(LetterProperties letter)
    {
        if (!wordToGenerate.Contains(letter.Character))
        {
            backGround.color = Color.red;
        }

        else
        {
            allTiles[wordToGenerate.IndexOf(letter.Character)].GetComponent<Image>().color = Color.green;
            ++correctInput;
        }
    }
}
54+?}65z