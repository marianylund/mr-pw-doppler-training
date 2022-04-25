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
        private float _samplingDepth = 0.5F;
        private float _maxPrf = 22f;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            _dopplerVisualiser = (DopplerVisualiser)target;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Arterial Velocity");
            _arterialVelocity = EditorGUILayout.Slider(_arterialVelocity, 0.0f, 3.0f);
            EditorGUILayout.LabelField("Pulse Repetition Frequency");
            _pulseRepetitionFrequency = EditorGUILayout.Slider(_pulseRepetitionFrequency, 1f, _maxPrf);
            EditorGUILayout.LabelField("Angle");
            _angle = EditorGUILayout.Slider(_angle, 15f, 90f);
            EditorGUILayout.LabelField("Sampling Depth");
            _samplingDepth = EditorGUILayout.Slider(_samplingDepth, 0.05f, 1f);

            
            if (EditorGUI.EndChangeCheck())
            {
                _dopplerVisualiser.Simulator.ArterialVelocity = _arterialVelocity;
                _dopplerVisualiser.Simulator.PulseRepetitionFrequency = _pulseRepetitionFrequency;
                _dopplerVisualiser.Simulator.Angle = _angle;
                _dopplerVisualiser.Simulator.SamplingDepth = _samplingDepth;
                
                _maxPrf = _dopplerVisualiser.Simulator.MaxPRF;
                _dopplerVisualiser.UpdateDoppler();
            }
        }
        
    }
}