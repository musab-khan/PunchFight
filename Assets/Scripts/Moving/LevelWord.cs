using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelWord: MonoBehaviour
{
    public string word;

    private void Start()
    {
        word = word.ToUpper();
    }
}
