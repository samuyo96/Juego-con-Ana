using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BlockDatabase/Decoration")]
public class DecorationDataBase : ScriptableObject
{
    public List<Decoration> decorations = new List<Decoration>();

    public Decoration FindDecoById(int id)
    {
        if (id > decorations.Count)
        {
            return null;
        }
        else
        {
            return decorations[id];
        }
    }

    public void StartAllBlock()
    {
        for (int n = 0; n < decorations.Count; n++)
        {
            for (int nBlock = 0; nBlock < decorations[n].models.Length; nBlock++)
            {
                if (decorations[n].models[nBlock] != null)
                {
                    Mesh mesh = decorations[n].models[nBlock].GetComponent<MeshFilter>().sharedMesh;
                    MeshRotator.RotateMesh(-90f, 0f, 0f, mesh);
                }
            }
            Debug.Log($"El bloque: {decorations[n].name} ha sido actualizado");
        }
    }
}

[System.Serializable]
public class Decoration
{
    public int id;
    public string name;
    public Block.DestructibleComp destrComp;
    public DecType decorationType;
    public GameObject[] models;

    public enum DecType
    {
        Grass = 0, Rock = 1, Road = 2, Stair = 3
    };
 
}
