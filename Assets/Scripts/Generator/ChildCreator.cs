using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChildCreator
{
    public static void GenerateBasic (Transform chunkParent, string name)
    {
        GameObject child = new GameObject();
        child.transform.parent = chunkParent;
        child.transform.position = chunkParent.position;
        child.name = name;
    }

    public static void GenerateComplex (Transform parent, TerrainGeneratorMaster.LayerData.LayerType type, int[,] typeMatrix, int[,] heightMatrix, Vector2Int chunkOffset)
    {
        GenerateBasic(parent, type.ToString());
        MeshFilter mf = parent.GetChild(parent.childCount - 1).gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        MeshRenderer mr = parent.GetChild(parent.childCount - 1).gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        Renderizer rd = parent.GetChild(parent.childCount - 1).gameObject.AddComponent(typeof(Renderizer)) as Renderizer;
        switch (type)
        {
            case TerrainGeneratorMaster.LayerData.LayerType.Terrain:
                StandardLayer sl = parent.GetChild(parent.childCount - 1).gameObject.AddComponent(typeof(StandardLayer)) as StandardLayer;
                sl.StartLayer(heightMatrix, typeMatrix, chunkOffset);
                break;
            case TerrainGeneratorMaster.LayerData.LayerType.Water:
                WaterLayer wl = parent.GetChild(parent.childCount - 1).gameObject.AddComponent(typeof(WaterLayer)) as WaterLayer;
                wl.StartLayer(heightMatrix, typeMatrix, chunkOffset);
                break;
            case TerrainGeneratorMaster.LayerData.LayerType.Grass:
                GrassLayer gl = parent.GetChild(parent.childCount - 1).gameObject.AddComponent(typeof(GrassLayer)) as GrassLayer;
                gl.StartLayer(heightMatrix, typeMatrix, chunkOffset);
                break;
            default:
                break;
        }
    }

    public static void GenerateSubLayer(Transform parent, GameVar.FloatingMesh fm, string name, Material material, UnityEngine.Rendering.ShadowCastingMode mode)
    {
        GenerateBasic(parent, name);
        MeshFilter mf = parent.GetChild(parent.childCount - 1).gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        MeshRenderer mr = parent.GetChild(parent.childCount - 1).gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        Renderizer rd = parent.GetChild(parent.childCount - 1).gameObject.AddComponent(typeof(Renderizer)) as Renderizer;
        SubLayer water = parent.GetChild(parent.childCount - 1).gameObject.AddComponent(typeof(SubLayer)) as SubLayer;
        water.StartLayer(fm, material, mode);
    }
}
