using UnityEngine;
using UnityEditor;

public class BuilderController : MonoBehaviour
{
    public Vector2Int startPosition;
    public int radious;
    public BuildMode mode;
    public BuildPowerUps up;

    public int macroX;
    public int macroZ;

    public enum BuildMode
    {
        BuildStructures = 0,
        ModTerrainUp = 1,
        ModTerrainDown = 2
    }

    public enum BuildPowerUps
    {
        None = 0,
        IgnoreDecoLow = 1,
        IgnoreDecoHigh = 2
    }

    [CustomEditor(typeof(BuilderController))]
    public class BuilderControllerEditor : Editor
    {
        BuilderController targetScript;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            targetScript = (BuilderController)target;
            GUILayout.Space(10);
            if (GUILayout.Button("SetUpObject"))
            {
                targetScript.SetUpObject();
            }
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Read"))
            {
                targetScript.Read();
            }
            if (GUILayout.Button("Build"))
            {
                targetScript.Build();
            }
            GUILayout.EndHorizontal();
        }
    }

    public void SetUpObject()
    {
        if (!TryGetComponent(out ModTerrain mod))
        {         
            ChildCreator.GenerateBasic(transform, "Map");
            ChildCreator.GenerateBasic(transform, "Grid");
            ModTerrain modT = gameObject.AddComponent(typeof(ModTerrain)) as ModTerrain;
            Reader read = transform.GetChild(0).gameObject.AddComponent(typeof(Reader)) as Reader;
        }
    }

    public void Read ()
    {
        transform.GetChild(0).GetComponent<Reader>().ReadMapHierarchy(macroX, macroZ);
    }

    public void Build()
    {

    }
}
