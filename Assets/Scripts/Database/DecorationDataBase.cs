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
        Mesh mesh = new Mesh ();
        for (int n = 0; n < decorations.Count; n++)
        {
            for (int nBlock = 0; nBlock < decorations[n].models.Length; nBlock++)
            {
                if (decorations[n].models[nBlock] != null)
                {
                    if (decorations[n].decorationType == Decoration.DecType.Tree)
                    {
                        for (int s = 0; s < decorations[n].models[nBlock].transform.childCount; s++)
                        {
                            mesh = decorations[n].models[nBlock].transform.GetChild(s).GetComponent<MeshFilter>().sharedMesh;
                            MeshRotator.RotateMesh(-90f, 0f, 0f, mesh);
                        }
                    }
                    else
                    {
                        mesh = decorations[n].models[nBlock].GetComponent<MeshFilter>().sharedMesh;
                        MeshRotator.RotateMesh(-90f, 0f, 0f, mesh);
                    }
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
    public float[] probability;

    public enum DecType
    {
        Grass = 0, Rock = 1, Road = 2, Stair = 3, Tree = 4
    };
 
}
