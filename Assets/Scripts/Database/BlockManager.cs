using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "BlockDatabase/Manager")]
public class BlockManager : ScriptableObject
{
    public List<BlockRelations> blockRelations;

    public BlockRelations FindBlockById(int id)
    {
        if (id > blockRelations.Count)
        {
            return null;
        }
        else
        {
            return blockRelations[id];
        } 
    }
}

[System.Serializable]
public class BlockRelations
{
    [System.Serializable]
    public class BlockDescriptor
    {
        public int id;

        public VoxelSubtype subtype;

        public enum VoxelSubtype
        {Default = 0, CornerA = 1, CornerB = 2, Double = 3, Triple = 4, Total = 5, Start = 6, End = 7};
    }
    public int id;
    public VoxelType type;
    public List<BlockDescriptor> blockDescriptors;
    public enum VoxelType
    { Plain = 0, GreatHillside = 1, HillsideMod = 2, SmallHillside = 3, Decoration = 4, Default = 10 };
}


