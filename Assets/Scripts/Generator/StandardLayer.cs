using UnityEngine;

public class StandardLayer : MonoBehaviour
{
    public int[][,] layerMatrix;
    public Vector2Int chunkOffset;

    private int chunkSize;

    private GameVar.FloatingMesh mesh;
    private Material mainMaterial;
    private BlockManager manager;
    private BlockDataBase blocks;
    private float uvOffset;

    public void StartLayer(int[,] heightMap, int[,] typeMap, Vector2Int offset)
    {
        layerMatrix = new int[][,] {
            heightMap, typeMap
        };
        chunkOffset = offset;
        LoadLayer();
        TranslateMatrix();
        transform.GetComponent<Renderizer>().RenderFloatingMesh(mesh, mainMaterial,
                                 UnityEngine.Rendering.ShadowCastingMode.TwoSided);
    }

    private void LoadLayer ()
    {
        chunkSize = layerMatrix[0].GetLength(0) - 2;
        manager = Resources.Load("Database/BlockManager", typeof(BlockManager)) as BlockManager;
        blocks = Resources.Load("Database/BlockDatabase", typeof(BlockDataBase)) as BlockDataBase;
        mainMaterial = Resources.Load($"Materials/tileset", typeof(Material)) as Material;
        uvOffset = 16f / mainMaterial.mainTexture.width;
    }

    public void TranslateMatrix ()
    {
        mesh = GameVar.FloatingMesh.GenerateMonoFloatingMesh();
        BlockRelations blockFamily;
        for (int x = 1; x < chunkSize + 1; x++)
        {
            for (int z = 1; z < chunkSize + 1; z++)
            {
                if (layerMatrix[1][x, z] != -1)
                {
                    blockFamily = manager.FindBlockById(layerMatrix[1][x, z]);
                    BlockSelector(blockFamily, new Vector3Int(x, layerMatrix[0][x, z], z));
                }
            }
        }
    }

    private void BlockSelector (BlockRelations blockFamily, Vector3Int offset)
    {
        GameVar.BlockData blockData = new GameVar.BlockData();
        Vector2Int offset2D = new Vector2Int(offset.x, offset.z);
        switch (blockFamily.type)
        {
            case BlockRelations.VoxelType.GreatHillside:
                blockData = TopologyCalculator.CalculateTopographyForHillsides
                            (NormalizedMatrixCutter(offset2D, 1));
                break;
            case BlockRelations.VoxelType.Plain:
                blockData.index = blockData.rotationY = 0;
                break;
            default:
                Debug.Log("Block type not supported by StandardLayer");
                break;
        }
        GameVar.Voxel voxel = GenerateVoxel(blockFamily, blockData);
        AddFLoatingMeshes(AddBlockToGeometry.AddSimpleBlockToGeometry
                    (new Vector3(offset.x + chunkOffset.x, offset.y, offset.z + chunkOffset.y),
                     voxel, mesh.layerVerts.Count, uvOffset));
    }

    private int[,] NormalizedMatrixCutter (Vector2Int offset, int h)
    {
        int[,] around = new int[3, 3];
        int n = 0;
        bool stop = false;
        for (int aroundX = 0; aroundX < 3; aroundX++)
        {
            if (stop)
            {
                break;
            }
            for (int aroundZ = 0; aroundZ <3; aroundZ++)
            {              
                n = layerMatrix[0][offset.x + aroundX - 1, offset.y + aroundZ - 1];
                around[aroundX, aroundZ] = Mathf.FloorToInt((float)n / (float)h);
                if (n > h)
                {
                    stop = true;
                    around = NormalizedMatrixCutter(offset, n);
                    break;
                }
            }
        }
        return around;
    }
    private GameVar.Voxel GenerateVoxel(BlockRelations blockFamily, GameVar.BlockData blockData)
    {
        GameVar.Voxel voxel = new GameVar.Voxel();
        int index = blockFamily.blockDescriptors[blockData.index].id;
        Block block = blocks.FindBlockById(index);
        voxel.model = block.models[blockData.rotationY];
        voxel.uvWeights = block.textureWeightsOverTen;
        return voxel;
    }

    private void AddFLoatingMeshes(GameVar.FloatingMesh externalFM)
    {
        mesh.layerNorms.AddRange(externalFM.layerNorms);
        mesh.layerTris.AddRange(externalFM.layerTris);
        mesh.layerUVs.AddRange(externalFM.layerUVs);
        mesh.layerVerts.AddRange(externalFM.layerVerts);
    }
}
