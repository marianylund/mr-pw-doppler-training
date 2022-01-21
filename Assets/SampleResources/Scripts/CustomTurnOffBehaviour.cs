/*===============================================================================
Copyright (c) 2021 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
===============================================================================*/

using UnityEngine;
using Vuforia;

public class CustomTurnOffBehaviour : MonoBehaviour
{
    public enum TurnOffMode
    {
        PlayModeAndDevice,
        PlayModeOnly,
        Neither
    }

    public TurnOffMode TurnOffRendering = TurnOffMode.PlayModeAndDevice;

    // Mesh and Renderer will be destroyed if "PlayModeAndDevice" is selected or if we're running in PlayMode
    // and only if the "Neither" option isn't set. Setting "Neither" will keep the Mesh and Renderer.
    bool DestroyTargetBehaviourMeshAndRenderer => TurnOffRendering != TurnOffMode.Neither &&
                                                     (TurnOffRendering == TurnOffMode.PlayModeAndDevice || 
                                                      Application.isEditor);

    void Awake()
    {
        if (!DestroyTargetBehaviourMeshAndRenderer) 
            return;
        
        var meshRenderer = GetComponent<MeshRenderer>();
        var meshFilter = GetComponent<MeshFilter>();

        if (meshRenderer)
            Destroy(meshRenderer);
        if (meshFilter)
            Destroy(meshFilter);
    }
}