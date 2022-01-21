using UnityEditor;

namespace DopplerSim
{
    [CustomEditor(typeof(DopplerVisualiser))]
    public class DopplerVisualiserInspector : Editor
    {
        private DopplerVisualiser _dopplerVisualiser;
        private float _arterialVelocity = 1.0f;
        private float _pulseRepetitionFrequency = 20;
        private float _angle = 45f;
        private float _samplingDepth = 3.0F;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            _dopplerVisualiser = (DopplerVisualiser)target;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Arterial Velocity");
            _arterialVelocity = EditorGUILayout.Slider(_arterialVelocity, 0.0f, 3.0f);
            EditorGUILayout.LabelField("Pulse Repetition Frequency");
            _pulseRepetitionFrequency = EditorGUILayout.Slider(_pulseRepetitionFrequency, 1f, 22f);
            EditorGUILayout.LabelField("Angle");
            _angle = EditorGUILayout.Slider(_angle, 15f, 90f);
            EditorGUILayout.LabelField("Sampling Depth");
            _samplingDepth = EditorGUILayout.Slider(_samplingDepth, 0.4f, 7f);

            
            if (EditorGUI.EndChangeCheck())
            {
                _dopplerVisualiser.Simulator.ArterialVelocity = _arterialVelocity;
                _dopplerVisualiser.Simulator.PulseRepetitionFrequency = _pulseRepetitionFrequency;
                _dopplerVisualiser.Simulator.Angle = _angle;
                _dopplerVisualiser.Simulator.SamplingDepth = _samplingDepth;
                _dopplerVisualiser.UpdateDoppler();
            }
        }
        
    }
}