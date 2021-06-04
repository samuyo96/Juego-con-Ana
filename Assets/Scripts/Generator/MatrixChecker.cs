using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MatrixChecker 
{
    public static GameVar.BlockData Check2Pos(int[,] aroundTypeMatrix, int checkIndex, int index, int n)
    {
        GameVar.BlockData bc = new GameVar.BlockData();
        Vector2Int ortoPos = MathGame.OrtoDirections2D[checkIndex];
        bc.rotationY = index;
        if (aroundTypeMatrix[ortoPos.x + 1, ortoPos.y + 1] == -1)
        {
            bc.index = n * 2;
        }
        else
        {
            bc.index = 1;
        }
        return bc;

    }

    public static GameVar.BlockData Check3Pos(int[,] matrix, int step, int startOrtoDir, int targetType)
    {
        int[] segment = new int[3];
        GameVar.BlockData bc = new GameVar.BlockData();
        Vector2Int checkPos = new Vector2Int();
        bc.rotationY = MathGame.CycleIndex(3, startOrtoDir, -1);
        bc.index = -1;
        int index = startOrtoDir * 2;
        int n;
        for (int p = 0; p < 3; p++)
        {
            n = 0;
            checkPos = MathGame.AllDirections[index];
            if (matrix[checkPos.x + 1, checkPos.y + 1] == targetType)
            {
                n = 1;
            }
            segment[p] = n;
            index = MathGame.CycleIndex(7, index, step);
        }
        GameVar.IntBool ib = CheckSegment(segment, true, 1);
        if (ib.boolean)
        {
            if (segment[0] == 1 && segment[2] == 1)
            {
                bc.index = 2;
                return bc;
            }
            bc.index = 0;
            bc.rotationY = MathGame.CycleIndex(3, startOrtoDir, -ib.number);
        }
        else
        {
            ib = CheckSegment(segment, false, 1);
            if (ib.boolean)
            {
                bc.index = 1;
                return bc;
            }
            bc.index = 3;
            bc.rotationY = Random.Range(0, 2);
        }
        return bc;
    }

    public static GameVar.IntBool CheckSegment(int[] segment, bool ortogonal, int ask)
    {
        GameVar.IntBool ib = new GameVar.IntBool();
        if (ortogonal)
        {
            for (int check = 0; check < 2; check++)
            {
                if (segment[check * 2] == ask)
                {
                    ib.boolean = true;
                    ib.number = check;
                    return ib;
                }
            }
        }
        else
        {
            if (segment[1] == ask)
            {
                ib.boolean = true;
                ib.number = 1;
                return ib;
            }
        }
        return ib;
    }

    public static GameVar.IntBool CheckPositions(int[,] matrix, bool ortogonal)
    {
        GameVar.IntBool ib = new GameVar.IntBool();
        Vector2Int[] directions;
        if (ortogonal)
        {
            directions = MathGame.OrtoDirections2D;
        }
        else
        {
            directions = MathGame.PeriDirections2D;
        }
        Vector2Int dir;
        for (int o = 0; o < 4; o++)
        {
            dir = directions[o];
            if (matrix[dir.x + 1, dir.y + 1] == 1)
            {
                ib.number = o;
                ib.boolean = true;
                break;
            }
        }
        return ib;
    }

    public static GameVar.IntBool LChecker(int[,] matrix, int index, GameVar.IntBool prev)
    {
        GameVar.IntBool ib = prev;
        if (LCheckerFor(matrix, 1, 2 * index))
        {
            ib.boolean = true;
            ib.number = index;
            return ib;
        }
        if (LCheckerFor(matrix, -1, 2 * index))
        {
            ib.boolean = true;
            ib.number = MathGame.CycleIndex(3, index, -1);
            return ib;
        }
        prev.boolean = false;
        return ib;
    }

    public static bool LCheckerFor(int[,] matrix, int step, int index)
    {
        Vector2Int dir;
        int auxIndex = index - step;
        for (int s = 0; s < 3; s++)
        {
            auxIndex = MathGame.CycleIndex(7, auxIndex, step);
            dir = MathGame.AllDirections[auxIndex];
            if (matrix[dir.x + 1, dir.y + 1] == 0)
            {
                return false;
            }
        }
        return true;
    }
}
