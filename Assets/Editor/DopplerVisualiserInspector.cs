using System;
using UnityEditor;
using UnityEngine;

namespace DopplerSim
{
    [CustomEditor(typeof(DopplerVisualiser))]
    public class DopplerVisualiserInspector : Editor
    {
        private DopplerVisualiser _dopplerVisualiser;
        private float _arterialVelocity = 1.0f * DopplerVisualiser.ConvertFromTrueToVisualised;
        private float _pulseRepetitionFrequency = 20 * DopplerVisualiser.ConvertFromTrueToVisualised;
        private float _angle = 45f;
        private float _samplingDepth = 0.5F;
        private float _maxPrf = 22f;
        private RaycastAngle raycastAngle;

        

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            _dopplerVisualiser = (DopplerVisualiser)target;
            
            raycastAngle = FindObjectOfType<RaycastAngle>();
            if (raycastAngle != null)
            {
                raycastAngle.valueUpdate += AngleUpdate;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Arterial Velocity");
            _arterialVelocity = EditorGUILayout.Slider(_arterialVelocity, 0.0f, _dopplerVisualiser.MaxArterialVelocity);
            EditorGUILayout.LabelField("Pulse Repetition Frequency");
            _pulseRepetitionFrequency = EditorGUILayout.Slider(_pulseRepetitionFrequency, 1f, _maxPrf);
            EditorGUILayout.LabelField("Angle");
            _angle = EditorGUILayout.Slider(_angle, 15f, 90f);
            EditorGUILayout.LabelField("Sampling Depth");
            _samplingDepth = EditorGUILayout.Slider(_samplingDepth, 0.05f, 1f);

            
            if (EditorGUI.EndChangeCheck())
            {
                _dopplerVisualiser.ArterialVelocity = _arterialVelocity;
                _dopplerVisualiser.PulseRepetitionFrequency = _pulseRepetitionFrequency;
                _dopplerVisualiser.Angle = _angle;
                _dopplerVisualiser.SamplingDepth = _samplingDepth;
                
                _maxPrf = _dopplerVisualiser.MaxPRF;
                _dopplerVisualiser.UpdateDoppler();
            }
        }

        private void OnDestroy()
        {
            if (raycastAngle != null)
            {
                raycastAngle.valueUpdate -= AngleUpdate;
            }
        }

        private void AngleUpdate(int newAngle, float overlap)
        {
            _dopplerVisualiser.Angle = raycastAngle.CurrentAngle;
            _dopplerVisualiser.Overlap = overlap;
            _dopplerVisualiser.UpdateDoppler();
        }
        
    }
}