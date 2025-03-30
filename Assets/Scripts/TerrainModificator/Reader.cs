using UnityEngine;

public class Reader : MonoBehaviour
{
    public int[,] macroMap;
    public int macroChunkSize;

    public void ReadMapHierarchy(int x, int z)
    {
        macroMap = new int[x, z];
        macroChunkSize = transform.GetChild(0).GetComponent<TerrainGeneratorMaster>().size;
        for (int i = 0; i < transform.childCount; i++)
        {
            TerrainGeneratorMaster tgm = transform.GetChild(i).GetComponent<TerrainGeneratorMaster>();
            macroMap[tgm.worldPos.x, tgm.worldPos.y] = i + 1;
        }
        for (int xf = 0; xf < x; xf++)
        {
            for (int zf = 0; zf < z; zf++)
            {
                macroMap[xf, zf]--;
            }
        }
    }
}
