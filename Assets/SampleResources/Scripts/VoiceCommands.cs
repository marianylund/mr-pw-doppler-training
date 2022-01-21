/*===============================================================================
Copyright (c) 2021 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
===============================================================================*/

using Microsoft.MixedReality.Toolkit;
using UnityEngine;

public class VoiceCommands : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
    }

    public void Reset()
    {
        SampleUtil.ResetObjectTracker();
    }

    public void ShowHandMesh()
    {
        var handTrackingProfile = CoreServices.InputSystem?.InputSystemProfile?.HandTrackingProfile;

        if (handTrackingProfile != null)
            handTrackingProfile.EnableHandMeshVisualization = true;
    }

    public void HideHandMesh()
    {
        var handTrackingProfile = CoreServices.InputSystem?.InputSystemProfile?.HandTrackingProfile;

        if (handTrackingProfile != null)
            handTrackingProfile.EnableHandMeshVisualization = false;
    }
}