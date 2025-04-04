using UnityEngine;
using UnityEditor;

public class Renderizer : MonoBehaviour
{
    public void RenderFloatingMesh (GameVar.FloatingMesh fm, Material material, UnityEngine.Rendering.ShadowCastingMode mode)
    {
        MeshFilterContoller(transform.GetComponent<MeshFilter>(), fm);
        RenderMesh(transform.GetComponent<MeshRenderer>(), material, mode);
    }

    private void MeshFilterContoller(MeshFilter mf, GameVar.FloatingMesh fm)
    {
        mf.sharedMesh = new Mesh();
        mf.sharedMesh.Clear();
        mf.sharedMesh.name = transform.name;
        mf.sharedMesh.SetVertices(fm.layerVerts);
        mf.sharedMesh.SetNormals(fm.layerNorms);
        mf.sharedMesh.SetTriangles(fm.layerTris, 0);
        mf.sharedMesh.SetUVs(0, fm.layerUVs);
        mf.sharedMesh.RecalculateBounds();
        string localPath = $"Assets/Prefabs/Results/Meshes/{transform.name}.asset";
        AssetDatabase.GenerateUniqueAssetPath(localPath);
        AssetDatabase.CreateAsset(mf.sharedMesh, localPath);
    }

    private void RenderMesh(MeshRenderer mr, Material material, UnityEngine.Rendering.ShadowCastingMode mode)
    {
        mr.material = material;
        mr.shadowCastingMode = mode;
    }
}
