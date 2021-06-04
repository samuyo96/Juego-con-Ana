using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixMixer
{
    public int[] idList;
    public float[] ratioList;

    public static MatrixMixer GenerateUniversalMatrixMixer (int[,] aroundTypeMatrix, List<int> monovalues)
    {
        int value;
        int index;
        MatrixMixer mix = new MatrixMixer();
        List<int> idList = new List<int>();
        List<float> ratio = new List<float>();
        Vector2Int dir;
        idList.Add(aroundTypeMatrix[1, 1]);
        ratio.Add(0.2f);
        for (int direction = 0; direction < 8; direction++)
        {
            dir = MathGame.AllDirections[direction];
            value = aroundTypeMatrix[dir.x + 1, dir.y + 1];
            if (!monovalues.Contains(value))
            {
                if (idList.Contains(value))
                {
                    index = idList.IndexOf(value);
                    ratio[index] += (0.1f);
                }
                else
                {
                    idList.Add(value);
                    ratio.Add(0.1f);
                }
            }
        }
        mix.idList = idList.ToArray();
        mix.ratioList = ratio.ToArray();
        return mix;
    }
}

