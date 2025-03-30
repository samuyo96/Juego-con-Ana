using UnityEngine;
using UnityEditor;

public class MasterHerarchyController : MonoBehaviour
{
    public int chunkSize;
    public int[,] map;
    [HideInInspector]
    public bool[] mod;

    public int editChunk;

    [CustomEditor(typeof(MasterHerarchyController))]
    public class MHCEditor : Editor
    {
        MasterHerarchyController targetScript;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            targetScript = (MasterHerarchyController)target;
            GUILayout.Space(10);
            if (GUILayout.Button("Switch chunk"))
            {
                targetScript.SwitchModChunk(targetScript.editChunk);
            }
        }
    }

    public void StartMHC(int size, int xSize, int zSize)
    {
        chunkSize = size;
        map = new int[xSize, zSize];
        mod = new bool[xSize * zSize];
        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                map[x, z] = x * zSize + z;
            }
        }
    }

    public void SwitchModChunk (int chunk)
    {
        mod[chunk] = mod[chunk] ^ true;
        Debug.Log($"El Chunk {chunk} ha cambiado a {mod[chunk]}");
    }
}
