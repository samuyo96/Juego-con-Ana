using UnityEngine;

public static class MathGame
{
    #region Directions
    //Direcciones
    //Ortogonales normalizadas
    public static readonly Vector2Int[] OrtoDirections2D = new Vector2Int[]
    {
        Vector2Int.up,
        Vector2Int.left,
        Vector2Int.down,
        Vector2Int.right
    };

    //Diagonales normalizadas
    public static readonly Vector2Int[] PeriDirections2D = new Vector2Int[]
    {
        new Vector2Int (-1, 1),
        new Vector2Int (-1, -1),
        new Vector2Int (1, -1),
        new Vector2Int (1, 1)
    };

    public static readonly Vector2Int[] AllDirections = new Vector2Int[]
    {
        Vector2Int.up,
        new Vector2Int (-1, 1),
        Vector2Int.left,
        new Vector2Int (-1, -1),
        Vector2Int.down,
        new Vector2Int (1, -1),
        Vector2Int.right,
        new Vector2Int (1, 1),
        Vector2Int.zero
    };

    public static readonly Vector2Int[] PositiveDir = new Vector2Int[]
    {
        Vector2Int.zero,
        Vector2Int.right,
        Vector2Int.one,
        Vector2Int.up,
    };
    #endregion

    public static Vector2Int EdgeChecker(Vector2 origin, int xLength, int zLength)
    {
        Vector2 size = new Vector2(xLength, zLength);
        float xtest = origin.x / (size.x / 2f);
        float ztest = origin.y / (size.y / 2f);
        Vector2Int result = Vector2Int.one;
        if (xtest > 1f)
        {
            result.x = -1;
        }
        if (ztest > 1)
        {
            result.y = -1;
        }
        return result;
    }

    public static int CycleIndex(int length, int index, int step)
    {
        int result = index + step;
        if (result > length)
        {
            return CycleIndex(length, 0, result - length - 1);
        }
        if (result < 0)
        {
            return CycleIndex(length, length, result + 1);
        }
        return result;
    }

    public static bool PosChecker(Vector2Int position, int xLength, int zLength)
    {
        if (position.x > xLength || position.x < 0)
        {
            return true;
        }
        else if (position.y > zLength || position.y < 0)
        {
            return true;
        }
        return false;
    }

    public static int coordController(int pos, int length, int edge)
    {
        int aux = 0;
        if (pos > length || pos < 0)
        {
            aux = pos + edge;
        }
        else
        {
            aux = pos;
        }
        return aux;
    }

    public static int MaxCalculator(int start, int size, int chunkSize)
    {
        if (start + chunkSize > size)
        {
            return size;
        }
        else
        {
            return start + chunkSize;
        }
    }

    public static Vector2 OrtoModdedOffsetForPair(int index)
    {
        if (IsPair(index))
        {
            return Vector2.right;
        }
        return Vector2.up;
    }

    public static bool IsPair(int number)
    {
        return (number % 2 == 0);
    }

    public static float PseudoRandomGenerator(int nSeed)
    {
        int l = nSeed.ToString().Length;
        int n = nSeed;
        if (l <= 4)
        {
            n += 1;
            n *= (int)Mathf.Pow(11, 4 - l);
        }
        string sn;
        char[] sc = new char[4];
        for (int a = 0; a < 3; a++)
        {
            n *= n;
            sn = n.ToString();
            for (int b = 0; b < 4; b++)
            {
                sc[3 - b] = sn[2 + b];
            }
            sn = new string(sc);
            n = int.Parse(sn.ToString());
            if (n < 1000)
            {
                break;
            }
        }
        float result = (float)n / 9999f;
        return result;
    }

    public static int PRGRange(int nSeed, int start, int end)
    {
        int d = end - start;
        int n = Mathf.RoundToInt(PseudoRandomGenerator(nSeed) * d);
        n += start;
        return n;
    }
}
