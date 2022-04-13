using System;
using System.ComponentModel;
using Microsoft.MixedReality.Toolkit.UI.HandCoach;
using UnityEngine;

public class InteractionsCoachHelper : MonoBehaviour
{
    [SerializeField] private HandInteractionHint handL;
    [SerializeField] private HandInteractionHint handR;
    [SerializeField] private HandInteractionHint probe;

    [SerializeField] private Transform handLtransform;
    [SerializeField] private Transform handRtransform;

    private void Awake()
    { 
    }

    /// <param name="anim">NearSelect, HandFlip, AirTap, Rotate, Move, PalmUp, Scroll</param>
    public void ShowHand(Vector3 pos, string anim = "NearSelect", bool rightHand = true)
    {
        var interaction = rightHand ? handR : handL;
        var objTransform = rightHand ? handRtransform : handLtransform;
        
        
        interaction.StopHintLoop();
        objTransform.position = pos;
        string handAnim = anim + (rightHand ? "_R" : "_L");
        interaction.AnimationState = handAnim;
        interaction.StartHintLoop();
    }

    public void ShowProbe(Vector3 pos)
    {
        probe.StopHintLoop();
        probe.transform.position = pos;
        probe.StartHintLoop();
    }

    public void StopHand(bool rightHand = true)
    {
        if(rightHand)
            handR.StopHintLoop();
        else
        {
            handL.StopHintLoop();
        }
    }

    public void StopProbe()
    {
        probe.StopHintLoop();
    }
}
