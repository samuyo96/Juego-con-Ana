using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLayer : MonoBehaviour
{
    public int[][,] layerMatrix;
    public Vector2Int chunkOffset;

    private int chunkSize;

    private GameVar.FloatingMesh[] meshes;
    private Material mainMaterial;
    private BlockManager manager;
    private BlockDataBase blocks;
    private float uvOffset;

    private bool destroy = false;
    private string[] materials;

    public void StartLayer(int[,] heightMap, int[,] typeMap, Vector2Int offset)
    {
        layerMatrix = new int[][,] {
            heightMap, typeMap
        };
        materials = new string[]
        {
            "WaterTest", "WaterFall01", "Waterfall02"
        };
        meshes = GameVar.FloatingMesh.GenerateMultiFloatingMesh(4);
        chunkOffset = offset;
        LoadLayer();
        TranslateMatrix();
        if (destroy)
        {
            DestroyImmediate(gameObject);
        } 
        else
        {
            GenerateSubLayer();
            transform.GetComponent<Renderizer>().RenderFloatingMesh(meshes[0], mainMaterial,
                                           UnityEngine.Rendering.ShadowCastingMode.TwoSided);
        }

    }

    private void LoadLayer()
    {
        chunkSize = layerMatrix[0].GetLength(0) - 2;
        manager = Resources.Load("Database/BlockManager", typeof(BlockManager)) as BlockManager;
        blocks = Resources.Load("Database/BlockDatabase", typeof(BlockDataBase)) as BlockDataBase;
        mainMaterial = Resources.Load($"Materials/tileset", typeof(Material)) as Material;
        uvOffset = 16f / mainMaterial.mainTexture.width;
    }

    public void TranslateMatrix()
    {
        BlockRelations blockFamily;
        int c = 0;
        for (int x = 1; x < chunkSize + 1; x++)
        {
            for (int z = 1; z < chunkSize + 1; z++)
            {
                if (layerMatrix[1][x, z] != -1)
                {
                    blockFamily = manager.FindBlockById(layerMatrix[1][x, z]);
                    BlockSelector(blockFamily, new Vector3Int(x, layerMatrix[0][x, z], z));
                }
                c += layerMatrix[1][x, z];
            }
        }
        destroy = (c == -1 * chunkSize * chunkSize);
    }

    private void GenerateSubLayer()
    {
        for (int subLayer = 1; subLayer < 4; subLayer++)
        {
            if (meshes[subLayer].layerVerts.Count != 0)
            {

                ChildCreator.GenerateSubLayer(transform, meshes[subLayer], $"waterSubLayer0{subLayer}",
                             Resources.Load($"Materials/{materials[subLayer-1]}", typeof(Material)) as Material,
                             UnityEngine.Rendering.ShadowCastingMode.Off);
            }
        }
    }

    private void BlockSelector (BlockRelations blockFamily, Vector3Int offset)
    {
        int secondaryDim = 0;
        GameVar.BlockData[] blockData = new GameVar.BlockData[0];
        Vector2Int offset2D = new Vector2Int(offset.x, offset.z);
        GameVar.BlockData waterBD = new GameVar.BlockData();
        waterBD.index = blockFamily.blockDescriptors.Count - 1;
        GameVar.Voxel waterVoxel = new GameVar.Voxel();
        bool small = false;
        switch (blockFamily.type)
        {
            case BlockRelations.VoxelType.SmallHillside:
                blockData = TopologyCalculator.CalculateTopographyForSmallHillsides
                            (MatrixCutter(offset2D, 1));
                secondaryDim = 1;
                small = true;
                break;
            case BlockRelations.VoxelType.HillsideMod:
                blockData = TopologyCalculator.CalculateTopographyForHillsideModded
                             (MatrixCutter(offset2D, 1), MatrixCutter(offset2D, 0));
                waterBD.rotationY = blockData[0].rotationY;
                if (MathGame.IsPair(waterBD.rotationY))
                {
                    secondaryDim = 2;
                }
                else
                {
                    secondaryDim = 3;
                }
                small = false;
                break;
            default:
                break;
        }
        waterVoxel = GenerateVoxel(blockFamily, waterBD);
        int[] vCount = new int[]
        {
            meshes[0].layerVerts.Count,
            meshes[secondaryDim].layerVerts.Count
        };
        GameVar.OptimizedMultiVoxel multiVoxel = GenerateMultiVoxel(blockFamily, blockData, small);
        GameVar.FloatingMesh[] auxFM = AddBlockToGeometry.AddMultiLayerBlockToGeometry(new Vector3Int
            (offset.x + chunkOffset.x, offset.y, offset.z + chunkOffset.y), multiVoxel, waterVoxel, vCount, uvOffset, small);
        for (int piece = 0; piece < blockData.Length; piece++)
        {
            AddFLoatingMeshes(auxFM[piece], 0);
        }
        AddFLoatingMeshes(auxFM[blockData.Length], secondaryDim);
    }

    private int[,] MatrixCutter(Vector2Int offset, int id)
    {
        int[,] around = new int[3, 3];
        for (int aroundX = 0; aroundX < 3; aroundX++)
        {
            for (int aroundZ = 0; aroundZ < 3; aroundZ++)
            {
                around[aroundX, aroundZ] = layerMatrix[id][offset.x + aroundX - 1, offset.y + aroundZ - 1];
            }
        }
        return around;
    }

    private GameVar.OptimizedMultiVoxel GenerateMultiVoxel(BlockRelations blockFamily, GameVar.BlockData[] blockData, bool small)
    {
        GameVar.OptimizedMultiVoxel optMultivoxel = GameVar.OptimizedMultiVoxel.CreateBlankMultiVoxel(blockData.Length);
        for (int pos = 0; pos < blockData.Length; pos++)
        {
            optMultivoxel.voxels[pos] = GenerateVoxel(blockFamily, blockData[pos]);
            if (small)
            {
                optMultivoxel.offset[pos] = new Vector3((float)(MathGame.PositiveDir[pos].x - 1) * 0.5f, 0f,
                                                        (float)(MathGame.PositiveDir[pos].y - 1) * 0.5f);
            }
            else
            {
                Vector2 modOffset = MathGame.OrtoModdedOffsetForPair(blockData[0].rotationY);
                optMultivoxel.offset[pos] = new Vector3(modOffset.x * 0.5f * (pos - 1), 0f,
                                                        modOffset.y * 0.5f * (pos - 1));
            }
            
        }
        return optMultivoxel;
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

    private void AddFLoatingMeshes(GameVar.FloatingMesh externalFM, int dimension)
    {
        meshes[dimension].layerNorms.AddRange(externalFM.layerNorms);
        meshes[dimension].layerTris.AddRange(externalFM.layerTris);
        meshes[dimension].layerUVs.AddRange(externalFM.layerUVs);
        meshes[dimension].layerVerts.AddRange(externalFM.layerVerts);
    }
}
