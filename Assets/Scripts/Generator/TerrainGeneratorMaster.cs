using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class TerrainGeneratorMaster : MonoBehaviour
{
    [System.Serializable]
    public class LayerData
    {
        public string nameOfLayer;
        public string nameOfBlockTypeCSV;
        public LayerType type;

        public enum LayerType
        {
            Default = 0,
            Terrain = 1, 
            Water = 2,
            Grass = 3
        }
    }
    public List<LayerData> layers;
    public string nameOfBlockHeightCSV;
    public int xSize;
    public int zSize;
    public int chunkSize;
    public BlockDataBase blocks;
    public DecorationDataBase deco;

    private int[][,] matrixArray;
    private bool layerGen;

    [CustomEditor(typeof(TerrainGeneratorMaster))]
    public class TerrainGeneratorMasterEditor : Editor
    {
        TerrainGeneratorMaster targetScript;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            targetScript = (TerrainGeneratorMaster)target;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate"))
            {
                targetScript.StartScript();
            }
            if (GUILayout.Button("Clear"))
            {
                targetScript.Clear();
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button("StartDatabase"))
            {
                targetScript.StartDatabase();
            }
        }
    }

    public void StartScript ()
    {
        int childCount;
        matrixArray = new int[2][,];
        matrixArray[0] = Reader(nameOfBlockHeightCSV);
        int[,] minitypeMatrix;
        for (int layer = 0; layer < layers.Count; layer++)
        {
            childCount = 0;
            matrixArray[1] = Reader(layers[layer].nameOfBlockTypeCSV);
            for (int x = 0; x < xSize; x += chunkSize)
            {
                for (int z = 0; z < zSize; z += chunkSize)
                {
                    if (childCount == transform.childCount)
                    {
                        ChildCreator.GenerateBasic(transform, $"chunk {x / chunkSize}, {z / chunkSize}");
                    }
                    minitypeMatrix = matrixCutterByCSize(1, x - 1, z - 1);
                    if (layerGen)
                    {
                        ChildCreator.GenerateComplex(transform.GetChild(childCount), layers[layer].type, minitypeMatrix, 
                                                     matrixCutterByCSize(0, x - 1, z - 1), new Vector2Int(x, z));
                    }
                    childCount++;
                }
            }
        }
    }

    public void Clear ()
    {
        int count = transform.childCount;
        for (int layer = 0; layer < count; layer++)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        matrixArray = new int[2][,];
    }

    public void StartDatabase ()
    {
        blocks.StartAllBlock();
        deco.StartAllBlock();
    }

    private int[,] Reader (string nameOfCSV)
    {
        int[,] matrix = new int[xSize, zSize];
        StreamReader strReader = new StreamReader($"Assets/CSV/{nameOfCSV}.csv");
        bool finish = false;
        int lineCounter = 0;
        while (!finish)
        {
            string line = strReader.ReadLine();
            if (line == null)
            {
                finish = true;
                break;
            }
            var lineSplited = line.Split(';');
            for (int l = 0; l < lineSplited.Length; l++)
            {
                matrix[lineCounter, l] = int.Parse(lineSplited[l]);
            }
            lineCounter++;
        }
        return matrix;
    }

    private int[,] matrixCutterByCSize(int id, int xStart, int zStart)
    {
        layerGen = true;
        int[,] minimatrix = new int[chunkSize + 2, chunkSize + 2];
        int c = 0;
        int value;  
        int auxX = 0;
        int auxZ = 0;
        Vector2Int edges = MathGame.EdgeChecker(new Vector2Int(xStart + 1, zStart + 1), xSize, zSize);
        for (int x = xStart; x < xStart + chunkSize + 2; x++)
        {
            auxX = MathGame.coordController(x, xSize - 1, edges.x);
            for (int z = zStart; z < zStart + chunkSize + 2; z++)
            {
                auxZ = MathGame.coordController(z, zSize - 1, edges.y);
                value = matrixArray[id][auxX, auxZ];
                minimatrix[x - xStart, z - zStart] = value;
                c += value;
            }
        }
        if (c == (-1 * (chunkSize + 2) * (chunkSize + 2)))
        {
            layerGen = false;
        }
        return minimatrix;
    }
}
