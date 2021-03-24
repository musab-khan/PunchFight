using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGenerator
{
    public static int GenerateInt(int min, int max)
    {
        return Random.Range(min, max);
    }

    public static float GenerateFloat(float min, float max)
    {
        return Random.Range(min, max);
    }
}
