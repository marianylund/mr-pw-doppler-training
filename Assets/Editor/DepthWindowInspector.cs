using UnityEditor;
using UnityEngine;

namespace DopplerSim
{
    [CustomEditor(typeof(DepthWindow))]
    public class DepthWindowInspector : Editor
    {
        private DepthWindow _depthWindow;
        private float _depthDebug;
        private float _windowSize = 0.0056f * 2;

        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            _depthWindow = (DepthWindow)target;


            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Depth");
            _depthDebug = EditorGUILayout.Slider(_depthDebug, _depthWindow.MixMaxDepth.x, _depthWindow.MixMaxDepth.y);
            
            EditorGUILayout.LabelField("Window Size");
            _windowSize = EditorGUILayout.Slider(_windowSize, _windowSize*0.3f, _windowSize*2);
            
            if (EditorGUI.EndChangeCheck())
            {
                _depthWindow.DepthDebug = _depthDebug;
                _depthWindow.WindowSize = _windowSize;
            }
        }
        

        
    }
}