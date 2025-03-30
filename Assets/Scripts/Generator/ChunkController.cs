using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkController : MonoBehaviour
{
    public int[,] chunkMatrix;
    public bool[,] editableChunkMatrix;

    public void StartController (int lengthX, int lengthZ, Vector2Int[] editableChunks)
    {
        chunkMatrix = new int[lengthX, lengthZ];
        editableChunkMatrix = new bool[lengthX, lengthZ];
        int n = 0;
        for (int x = 0; x < lengthX; x++)
        {
            for (int z = 0; z < lengthZ; z++)
            {
                chunkMatrix[x, z] = n++;
            }
        }
        if (editableChunks.Length != 0)
        {
            for (int b = 0; b < editableChunkMatrix.Length; b++)
            {
                editableChunkMatrix[editableChunks[b].x, editableChunks[b].y] = true;
            }
        }
    }
}
