using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubLayer : MonoBehaviour
{
    public void StartLayer(GameVar.FloatingMesh fm, Material material, UnityEngine.Rendering.ShadowCastingMode mode)
    {
        transform.GetComponent<Renderizer>().RenderFloatingMesh(fm, material, mode);
    }
}
