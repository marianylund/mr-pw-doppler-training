using System;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class DepthWindow : MonoBehaviour
{

    [SerializeField] private Transform top;
    [SerializeField] private Transform bottom;
    
    /// <summary>
    /// The same as art_overlap_total value in Doppler calculation.
    /// Between 0 and 1.
    /// </summary>
    public float Overlap { get; private set; }
    
    public float WindowSize
    {
        get => _windowSize;
        set
        {
            top.localPosition = new Vector3(top.localPosition.x, top.localPosition.y, -value / 2);
            bottom.localPosition = new Vector3(bottom.localPosition.x, bottom.localPosition.y, value / 2);
            _windowSize = value;
        }
    }

    public float DepthDebug
    {
        get => _depthDebug;
        set
        {
            window.localPosition = startDepth + new Vector3(0, 0, value);

            _depthDebug = value;
        }
    }

    public Vector2 MixMaxDepth { get; } = new Vector2(-0.1f, 0f);

    private float _windowSize = 0.0056f * 2;

    private Transform window;
    private float _depthDebug = 0f;
    private Vector3 startDepth;

    void Start()
    {
        window = bottom.parent;
        startDepth = window.localPosition;
    }

    public void UpdateDepthFromSlider(SliderEventData data)
    {
        DepthDebug = Mathf.Lerp( MixMaxDepth.x, MixMaxDepth.y, data.NewValue);
    }

    /// <summary>
    /// Calculates distance to each point and compares them to see if some of the points are inside to calculate the overlap
    /// </summary>
    /// <param name="i1">Point of hit in world position</param>
    /// <param name="i2">Point of hit backwards in world position</param>
    public float CalculateOverlap(Vector3 i1, Vector3 i2)
    {

        Vector3 p1 = top.position;
        Vector3 p2 = bottom.position;
        
        var pos = transform.position;
        
        float distToP1 = Vector3.Distance(pos, p1);
        float distToP2 = Vector3.Distance(pos, p2);

        float distToI1 = Vector3.Distance(pos, i1);
        float distToI2 = Vector3.Distance(pos, i2);
        
        bool p1Outside = (distToP1 < distToI1 && distToP1 < distToI2) || (distToP1 > distToI1 && distToP1 > distToI2);
        bool p2Outside = (distToP2 < distToI1 && distToP2 < distToI2) || (distToP2 > distToI1 && distToP2 > distToI2);
        
        bool p1Inside = distToP1 >= distToI1 && distToP1 <= distToI2;
        bool p2Inside = distToP2 >= distToI1 && distToP2 <= distToI2;

        float overlap = 0f;
        if (p1Outside && p2Outside)
        {
            overlap = 0f;
        } else if (p1Inside && p2Inside)
        {
            overlap = 1f;
        }
        else
        {
            if (p1Inside && p2Outside)
            {
                overlap = (distToI2 - distToP1)/_windowSize;
            }
            else if(p1Outside && p2Inside)
            {
                overlap = (distToP2 - distToI1)/_windowSize;
            }
        }
        
        //Vector3 localForward = transform.TransformDirection(Vector3.forward);
        //Debug.DrawRay(pos, localForward * distToP1, Color.magenta);
        //Debug.DrawRay(bottom.position, -localForward * distToP2, Color.green);

        Overlap = overlap;
        return 1.0f;//overlap;
    }
}
