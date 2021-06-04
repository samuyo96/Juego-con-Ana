using System.Collections.Generic;
using UnityEngine;

public class GameVar
{
    public class BlockData
    {
        public int index;
        public int rotationY;
    }

    public class IntBool
    {
        public int number;
        public bool boolean = false;

        public static IntBool CreateIntBool(int number, bool boolean)
        {
            IntBool ib = new IntBool();
            ib.number = number;
            ib.boolean = boolean;
            return ib;
        }
    }

    public class Voxel
    {
        public GameObject model;
        public float[] uvWeights;
    }

    public class MultiVoxel
    {
        public List<GameObject> models;
        public List<float[]> uvWeights;
        public List<Vector3> offset;

        public static MultiVoxel CreateBlankMultiVoxel()
        {
            MultiVoxel mv = new MultiVoxel();
            mv.models = new List<GameObject>();
            mv.uvWeights = new List<float[]>();
            mv.offset = new List<Vector3>();
            return mv;
        }
    }

    public class OptimizedMultiVoxel
    {
        public Voxel[] voxels;
        public Vector3[] offset;

        public static OptimizedMultiVoxel CreateBlankMultiVoxel(int dimension)
        {
            OptimizedMultiVoxel mv = new OptimizedMultiVoxel();
            mv.voxels = new Voxel[dimension];
            mv.offset = new Vector3[dimension];
            return mv;
        }
    }

    public class FloatingMesh
    {
        public List<Vector3> layerVerts = new List<Vector3>();
        public List<Vector3> layerNorms = new List<Vector3>();
        public List<int> layerTris = new List<int>();
        public List<Vector2> layerUVs = new List<Vector2>();

        public static FloatingMesh[] GenerateMultiFloatingMesh(int dimension)
        {
            FloatingMesh[] fm = new FloatingMesh[dimension];
            for (int dim = 0; dim < dimension; dim++)
            {
                fm[dim] = GenerateMonoFloatingMesh();
            }
            return fm;
        }

        public static FloatingMesh GenerateMonoFloatingMesh ()
        {
            FloatingMesh fm = new FloatingMesh();
            fm.layerVerts = new List<Vector3>();
            fm.layerNorms = new List<Vector3>();
            fm.layerTris = new List<int>();
            fm.layerUVs = new List<Vector2>();
            return fm;
        }
    }
}
