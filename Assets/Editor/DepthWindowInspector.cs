using UnityEditor;
using UnityEngine;

namespace DopplerSim
{
    [CustomEditor(typeof(DepthWindow))]
    public class DepthWindowInspector : Editor
    {
        private DepthWindow _dopplerVisualiser;
        private float _depthDebug;
        private float _windowSize = 0.0056f * 2;
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            _dopplerVisualiser = (DepthWindow)target;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Depth");
            _depthDebug = EditorGUILayout.Slider(_depthDebug, 0f, -0.1f);
            
            EditorGUILayout.LabelField("Window Size");
            _windowSize = EditorGUILayout.Slider(_windowSize, _windowSize*0.3f, _windowSize*2);
            
            if (EditorGUI.EndChangeCheck())
            {
                _dopplerVisualiser.DepthDebug = _depthDebug;
                _dopplerVisualiser.WindowSize = _windowSize;
            }
        }
        
    }
}