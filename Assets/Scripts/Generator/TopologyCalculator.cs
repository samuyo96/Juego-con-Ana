using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TopologyCalculator
{
    public static GameVar.BlockData CalculateTopographyForHillsides (int[,] aroundTypeMatrix)
    {
        GameVar.BlockData bc = new GameVar.BlockData();
        GameVar.IntBool rootDirection = MatrixChecker.CheckPositions(aroundTypeMatrix, true);
        if (rootDirection.boolean)
        {
            rootDirection = MatrixChecker.LChecker(aroundTypeMatrix, rootDirection.number, rootDirection);
            if (rootDirection.boolean)
            {
                bc.index = 1;
                bc.rotationY = rootDirection.number;
            }
            else
            {
                bc.index = 0;
                bc.rotationY = rootDirection.number;
            }
        }
        else
        {
            rootDirection = MatrixChecker.CheckPositions(aroundTypeMatrix, false);
            if (rootDirection.boolean)
            {
                bc.index = 2;
                bc.rotationY = rootDirection.number;
            }
            else
            {
                Debug.Log("ERROR");
            }
        }
        return bc;
    }

    public static GameVar.BlockData[] CalculateTopographyForSmallHillsides(int[,] aroundTypeMatrix)
    {
        GameVar.BlockData[] bc = new GameVar.BlockData[4];
        for (int n = 0; n < 4; n++)
        {
            bc[n] = MatrixChecker.Check3Pos(aroundTypeMatrix, -1, n, -1);
        }
        return bc;
    }
    public static GameVar.BlockData[] CalculateTopographyForHillsideModded(int[,] aroundTypeMatrix, int[,] aroundMatrix)
    {
        GameVar.BlockData[] bc = new GameVar.BlockData[2];
        int index = FindOrtoIndex(aroundMatrix);
        if (index != -1)
        {
            int checkIndex = index;
            for (int n = 0; n < 2; n++)
            {
                checkIndex = MathGame.CycleIndex(3, checkIndex, -1 * (n + 1));
                bc[n] = MatrixChecker.Check2Pos(aroundTypeMatrix, checkIndex, index, n);
            }
        }
        return bc;
    }

    private static int FindOrtoIndex(int[,] aroundMatrix)
    {
        int h = aroundMatrix[1,1];
        int checkh;
        Vector2Int ortoVector;
        for (int o = 0; o < 4; o++)
        {
            ortoVector = MathGame.OrtoDirections2D[o];
            checkh = aroundMatrix[ortoVector.x + 1, ortoVector.y + 1];
            if (checkh > h)
            {
                return o;
            }
        }
        return -1;
    }
}
