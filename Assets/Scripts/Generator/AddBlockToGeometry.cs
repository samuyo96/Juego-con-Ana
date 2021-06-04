using UnityEngine;

public static class AddBlockToGeometry
{
    public static GameVar.FloatingMesh[] AddMultiLayerBlockToGeometry (Vector3Int offset, GameVar.OptimizedMultiVoxel multiVoxel, GameVar.Voxel voxel, int[] vCount, float uvOffset, bool microDisplacement)
    {
        int nLayer = multiVoxel.voxels.Length;
        float microDisp = 0;
        if (microDisplacement)
        {
            int r = Random.Range(0, 2);
            microDisp = -((float)r * (uvOffset / 2));
        }
        int extraVCount = 0;
        GameVar.FloatingMesh[] fm = GameVar.FloatingMesh.GenerateMultiFloatingMesh(nLayer + 1);
        for (int subBlock = 0; subBlock < nLayer; subBlock++)
        {
            fm[subBlock] = TransformMeshToLists(offset - multiVoxel.offset[subBlock], multiVoxel.voxels[subBlock],
                           vCount[0] + extraVCount, uvOffset, microDisp);
            extraVCount += fm[subBlock].layerVerts.Count;
        }
        fm[nLayer] = TransformMeshToListProcedural(offset, voxel, vCount[1]);
        return fm;
    }

    public static GameVar.FloatingMesh AddSimpleBlockToGeometry(Vector3 offset, GameVar.Voxel voxel, int vCount, float uvOffset)
    {
        float uvDisplace = 0f;
        if (uvOffset != 0)
        {
            uvDisplace = MoveUV(voxel.uvWeights, uvOffset);
        }
        GameVar.FloatingMesh fm = TransformMeshToLists(offset, voxel, vCount, uvDisplace, 0);
        return fm;
    }

    private static GameVar.FloatingMesh TransformMeshToLists (Vector3 offset, GameVar.Voxel voxel, int vCount, float uvDisplace, float microdisplacement)
    {
        GameVar.FloatingMesh fm = GameVar.FloatingMesh.GenerateMonoFloatingMesh();
        Vector2 uvPos = new Vector2();
        if (voxel.model.GetComponent<MeshFilter>().sharedMesh != null)
        {
            Mesh blockMesh = voxel.model.GetComponent<MeshFilter>().sharedMesh;
            for (int v = 0; v < blockMesh.vertexCount; v++)
            {
                fm.layerVerts.Add(blockMesh.vertices[v] + offset);
                fm.layerNorms.Add(blockMesh.normals[v]);
                uvPos = blockMesh.uv[v];
                uvPos.Set(uvPos.x - uvDisplace + microdisplacement, uvPos.y);
                fm.layerUVs.Add(uvPos);
            }
            for (int tris = 0; tris < blockMesh.triangles.Length; tris++)
            {
                fm.layerTris.Add(blockMesh.triangles[tris] + vCount);
            }
        }
        return fm;
    }

    private static GameVar.FloatingMesh TransformMeshToListProcedural (Vector3 offset, GameVar.Voxel voxel, int vCount)
    {
        GameVar.FloatingMesh fm = GameVar.FloatingMesh.GenerateMonoFloatingMesh();
        Vector2 uvPos = new Vector2();
        float waterUVChunkSize = 129f;
        if (voxel.model.GetComponent<MeshFilter>().sharedMesh != null)
        {
            Mesh blockMesh = voxel.model.GetComponent<MeshFilter>().sharedMesh;
            for (int v = 0; v < blockMesh.vertexCount; v++)
            {
                fm.layerVerts.Add(blockMesh.vertices[v] + offset);
                fm.layerNorms.Add(blockMesh.normals[v]);
                uvPos = blockMesh.uv[v];
                uvPos.Set((uvPos.x + offset.z) / waterUVChunkSize, ((-1 * uvPos.y) + offset.x + 1) / (waterUVChunkSize));
                fm.layerUVs.Add(uvPos);
            }
            for (int tris = 0; tris < blockMesh.triangles.Length; tris++)
            {
                fm.layerTris.Add(blockMesh.triangles[tris] + vCount);
            }
        }
        return fm;
    }

    private static float MoveUV(float[] weights, float uvOffset)
    {
        float p = Random.Range(0f, 1f);
        int n;
        if (p < weights[0] / 10f)
        {
            n = 0;
        }
        else if (p < (weights[0] + weights[1]) / 10f)
        {
            n = 1;
        }
        else
        {
            n = 2;
        }
        return n * uvOffset;
    }
}
