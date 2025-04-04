using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GrassLayer : MonoBehaviour
{
    public int[][,] layerMatrix;
    public Vector2Int chunkOffset;

    private int chunkSize;

    private GameVar.FloatingMesh[] meshes;

    private BlockManager manager;
    private Material mainMaterial;
    private DecorationDataBase decoration;
    private int[][] randomPos;

    private List<int> monoValues;

    private string[] tags;

    public void StartLayer(int[,] heightMap, int[,] typeMap, Vector2Int offset)
    {
        layerMatrix = new int[][,] {
            heightMap, typeMap
        };
        tags = new string[]
        {
            "RockDeco", "Other"
        };
        meshes = GameVar.FloatingMesh.GenerateMultiFloatingMesh(5);
        chunkOffset = offset;
        LoadLayer();
        TranslateMatrix();
        transform.GetComponent<Renderizer>().RenderFloatingMesh(meshes[0], mainMaterial,
                                 UnityEngine.Rendering.ShadowCastingMode.On);
        GenerateSubLayer();
        layerMatrix = new int[0][,];
    }

    private void LoadLayer()
    {
        monoValues = new List<int>();
        chunkSize = layerMatrix[0].GetLength(0) - 2;
        manager = Resources.Load("Database/DecoManager", typeof(BlockManager)) as BlockManager;
        decoration = Resources.Load("Database/DecoDatabase", typeof(DecorationDataBase)) as DecorationDataBase;
        mainMaterial = Resources.Load($"Materials/Grass", typeof(Material)) as Material;
        randomPos = new int[5][];
        for (int array = 0; array < 4; array++)
        {
            randomPos[array] = RandomPositionCalculator(5);
        }
        randomPos[4] = ModifiedRandomPosCalc(4);
    }

    private void TranslateMatrix ()
    {
        for (int x = 1; x < chunkSize + 1; x++)
        {
            for (int z = 1; z < chunkSize + 1; z++)
            {
                if (layerMatrix[1][x,z] != -1)
                {
                    MatrixMixer mix = MatrixMixer.GenerateUniversalMatrixMixer(MatrixCutter(new Vector2Int(x, z), 1), monoValues);
                    TranslateMixer(mix, new Vector3(x + chunkOffset.x, layerMatrix[0][x, z], z + chunkOffset.y));
                }
            }
        }
        monoValues.Clear();
    }

    private void GenerateSubLayer()
    {
        for (int subLayer = 1; subLayer < 3; subLayer++)
        {
            if (meshes[subLayer].layerVerts.Count != 0)
            {
                ChildCreator.GenerateSubLayer(transform, meshes[subLayer], $"GrassSubLayer0{subLayer} {transform.name}",
                    Resources.Load($"Materials/StaticDecoration", typeof(Material)) as Material, UnityEngine.Rendering.ShadowCastingMode.On, tags[subLayer -1]);
            }
        }
        GenerateTrees();
    }

    private void GenerateTrees ()
    {
        if (meshes[3].layerVerts.Count != 0)
        {
            ChildCreator.GenerateSubLayer(transform, meshes[3], $"TreeSubLayer {transform.name}",
            Resources.Load($"Materials/StaticDecoration", typeof(Material)) as Material, UnityEngine.Rendering.ShadowCastingMode.On, "TreeDeco");
            ChildCreator.GenerateSubLayer(transform.GetChild(transform.childCount -1), meshes[4], $"TreeInfraLayer {transform.name}",
            mainMaterial, UnityEngine.Rendering.ShadowCastingMode.On, "TreeDeco");
        }
    }

    private void TranslateMixer (MatrixMixer mix, Vector3 offset)
    {
        int r = Random.Range(0, 4);
        int index = 0;
        int counter = 0;
        int ratio = 0;
        BlockRelations blockFamily;
        Decoration decorationObj;
        Vector3 offsetMod;
        for (int mixIndex = 0; mixIndex < mix.idList.Length; mixIndex++)
        {
            blockFamily = manager.FindBlockById(mix.idList[mixIndex]);
            decorationObj = decoration.FindDecoById(blockFamily.blockDescriptors[0].id);
            ratio = Mathf.FloorToInt(mix.ratioList[mixIndex] * 5f);
            for (int n = 0; n < ratio; n++)
            {
                index = randomPos[r][counter];
                offsetMod = MiniDisplacementCalculator(index, blockFamily.type);
                BlockSelector(decorationObj, offset - offsetMod);
                counter++;
            }
        }
    }

    private void BlockSelector (Decoration decoration, Vector3 offset)
    {
        GameVar.Voxel voxel = new GameVar.Voxel();
        int layerIndex = 0;
        float angle = 0f;
        switch (decoration.decorationType)
        {
            case Decoration.DecType.Grass:
                voxel = GenerateSimpleVoxel(decoration.models[0]);
                angle = Random.Range(0f, 30f);
                break;
            case Decoration.DecType.Rock:
                voxel = GenerateSimpleVoxel(decoration.models[0]);
                layerIndex = 1;
                angle = Random.Range(0f, 45f);
                break;
            case Decoration.DecType.Tree:
                GameObject tree = modelSelectorForTrees(decoration);
                angle = Random.Range(0f, 180f);
                voxel = GenerateSimpleVoxel(tree.transform.GetChild(0).gameObject);
                AddFLoatingMeshes(AddBlockToGeometry.AddSimpleBlockToGeometry(Vector3.zero, voxel,
                                             meshes[4].layerVerts.Count, 0), 4, offset, angle);
                voxel = GenerateSimpleVoxel(tree.transform.GetChild(1).gameObject);
                layerIndex = 3;
                break;
            default:
                break;
        }
        AddFLoatingMeshes(AddBlockToGeometry.AddSimpleBlockToGeometry(Vector3.zero, voxel, 
                                             meshes[layerIndex].layerVerts.Count, 0), layerIndex, offset, angle);
    }

    private int[,] MatrixCutter(Vector2Int offset, int id)
    {
        monoValues.Clear();
        monoValues.Add(-1);
        int value;
        int[,] around = new int[3, 3];
        for (int aroundX = 0; aroundX < 3; aroundX++)
        {
            for (int aroundZ = 0; aroundZ < 3; aroundZ++)
            {
                value = layerMatrix[id][offset.x + aroundX - 1, offset.y + aroundZ - 1];
                around[aroundX, aroundZ] = value;
                if (!monoValues.Contains(value))
                {
                    if (manager.FindBlockById(value).type != BlockRelations.VoxelType.Default)
                    {
                        monoValues.Add(value);
                    }
                }
            }
        }
        return around;
    }

    private void AddFLoatingMeshes(GameVar.FloatingMesh externalFM, int dimension, Vector3 offset, float angle)
    {
        meshes[dimension].layerNorms.AddRange(externalFM.layerNorms);
        meshes[dimension].layerTris.AddRange(externalFM.layerTris);
        meshes[dimension].layerUVs.AddRange(externalFM.layerUVs);
        meshes[dimension].layerVerts.AddRange(MeshRotator.RotateVertexAroundVector
                          (Vector3.up, angle, externalFM.layerVerts, offset));
    }

    private GameVar.Voxel GenerateSimpleVoxel(GameObject model)
    {
        GameVar.Voxel voxel = new GameVar.Voxel();
        voxel.model = model;
        voxel.uvWeights = new float[] { 0f, 0f, 0f };
        return voxel;
    }

    private Vector3 MiniDisplacementCalculator (int index, BlockRelations.VoxelType type)
    {
        Vector3 epsilonVect = new Vector3(randomPos[0][index] - 2f, 1f, randomPos[2][index] - 2f);
        if (type == BlockRelations.VoxelType.Default || type == BlockRelations.VoxelType.Plain)
        {
            epsilonVect.Set(epsilonVect.x, randomPos[1][index] * 2f, epsilonVect.z);
        }
        epsilonVect *= 0.025f;
        Vector3 result = new Vector3();
        if (index < 4)
        {
            result.Set((float)(MathGame.PositiveDir[index].x - 1) * 0.5f, 0f,
                       (float)(MathGame.PositiveDir[index].y - 1) * 0.5f);
            return result + epsilonVect;
        }
        result.Set(-0.25f, 0f, -0.25f);
        return result + epsilonVect;
    }

    private int[] RandomPositionCalculator (int length)
    {
        List<int> position = new List<int>();
        for (int number = 0; number < length; number++)
        {
            position.Add(number);
        }
        int[] result = new int[length];
        int r;
        for (int pos = 0; pos < length; pos++)
        {
            r = Random.Range(0, position.Count);
            result[pos] = position[r];
            position.RemoveAt(r);
        }
        return result;
    }

    private int[] ModifiedRandomPosCalc (int length)
    {
        List<int> startNum = new List<int>();
        for (int number = 4; number > length - 1; number--)
        {
            startNum.Add(number);
        }
        List<int> endNums = RandomPositionCalculator(length).ToList();
        startNum.AddRange(endNums);
        return startNum.ToArray();
    }

    private GameObject modelSelectorForTrees (Decoration decoration)
    {
        float r = Random.Range(0f, 10f);
        int index = 0;
        float sum = 0;
        for (int pos = 0; pos < decoration.probability.Length; pos++)
        {
            sum += decoration.probability[pos];
            if (r < sum)
            {
                index = pos;
                break;
            }
        }
        return decoration.models[index];
    }
}
