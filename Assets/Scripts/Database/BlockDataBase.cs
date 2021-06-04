using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BlockDatabase/Database")]
public class BlockDataBase : ScriptableObject 
{
    public List<Block> blocks = new List<Block>();

    public Block FindBlockById (int id)
    {
        if (id > blocks.Count)
        {
            return null;
        }
        else
        {
            return blocks[id];
        }
    }

    public void StartAllBlock()
    {
        for (int n = 0; n < blocks.Count; n++)
        {
            for (int nBlock = 0; nBlock < blocks[n].models.Length; nBlock++)
            {
                if (blocks[n].models[nBlock] != null)
                {
                    Mesh mesh = blocks[n].models[nBlock].GetComponent<MeshFilter>().sharedMesh;
                    MeshRotator.RotateMesh(-90f, 0f, 0f, mesh);
                }
            }
            Debug.Log($"El bloque: {blocks[n].name} ha sido actualizado");
        }
    }
}

[System.Serializable]
public class Block
{
    public int id;
    public string name;
    public DestructibleComp destrComp;
    public BlockType blockType;
    public GameObject[] models;
    [HideInInspector]
    public float[] textureWeightsOverTen;
    public enum DestructibleComp { Indestructible = 0, Mod = 1, Destructible = 2 }
    public enum BlockType {None = 0, Wall = 1, Hillside = 2, Plain = 3, Carpet = 4 }
}

public static class MeshRotator
{
    public static Mesh RotateMesh(float angleX, float angleY, float angleZ, Mesh mesh)
    {
        if (angleX != 0)
        {
            mesh = RotateMeshAroundVector(Vector3.right, angleX, mesh);
        }
        if (angleY != 0)
        {
            mesh = RotateMeshAroundVector(Vector3.up, angleY, mesh);
        }
        if (angleZ != 0)
        {
            mesh = RotateMeshAroundVector(Vector3.forward, angleZ, mesh);
        }
        return mesh;
    }

    private static Mesh RotateMeshAroundVector(Vector3 axis, float angle, Mesh mesh)
    {
        Vector3[] vertexRotated = new Vector3[mesh.vertexCount];
        Quaternion angleQ = Quaternion.AngleAxis(angle, axis);
        for (int vert = 0; vert < mesh.vertexCount; vert++)
        {
            vertexRotated[vert] = angleQ * mesh.vertices[vert];
        }
        mesh.vertices = vertexRotated;
        return mesh;
    }

    public static List<Vector3> RotateVertexAroundVector(Vector3 axis, float angle, List<Vector3> vertexList, Vector3 offset)
    {
        List<Vector3> vertex = new List<Vector3>();
        Quaternion angleQ = Quaternion.AngleAxis(angle, axis);
        for (int vert = 0; vert < vertexList.Count; vert++)
        {
            vertex.Add(angleQ * vertexList[vert] + offset);
        }
        return vertex;
    }
}

