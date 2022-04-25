using DopplerSim;
using UnityEngine;

public class DopplerUI : MonoBehaviour
{
    [SerializeField]
    private SimpleSliderBehaviour bloodVelocitySlider;
    [SerializeField]
    private SimpleSliderBehaviour prfSlider;
    [SerializeField]
    private SimpleSliderBehaviour depthSlider;

    [SerializeField] private DopplerVisualiser _dopplerVisualiser;
    [SerializeField] RaycastAngle _raycastAngle;

    private float _timer = 0f;
    public void SetUp()
    {
        //Debug.Assert(prfSlider != null, "prfSlider is not set up in DopplerUI on " + gameObject.name);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (_dopplerVisualiser == null || _raycastAngle == null || bloodVelocitySlider == null || prfSlider == null ||
            depthSlider == null)
        {
            Debug.LogError("Values are not set up correctly on DopplerUI");
            this.enabled = false;
        }
        
        
        
        // _dopplerVisualiser.Simulator.ArterialVelocity = bloodVelocitySlider.CurrentValue;
        // _dopplerVisualiser.Simulator.SamplingDepth = depthSlider.CurrentValue;
        // _dopplerVisualiser.Simulator.PulseRepetitionFrequency = prfSlider.CurrentValue;
        // _dopplerVisualiser.Simulator.Angle = _raycastAngle.CurrentAngle;

        bloodVelocitySlider.valueUpdate += BloodVelocitySliderUpdate;
        prfSlider.valueUpdate += PRFSliderUpdate;
        depthSlider.valueUpdate += SamplingDepthSliderUpdate;
        _raycastAngle.valueUpdate += AngleUpdate;
    }

    private void BloodVelocitySliderUpdate()
    {
        _dopplerVisualiser.Simulator.ArterialVelocity = bloodVelocitySlider.CurrentValue;
        _dopplerVisualiser.UpdateDoppler();
    }

    private void SamplingDepthSliderUpdate()
    {
        // Depth is weird and needs to be 0.05 and 1.0 where around av_depth / 7.0D + 0.0125D + 0.05D
        // If av_depth is 3.0, so optimal depth is around 0.49107142857 is the optimum
        _dopplerVisualiser.Simulator.SamplingDepth = Mathf.Clamp(prfSlider.CurrentRawValue, 0.05f, 1.0f);
        _dopplerVisualiser.UpdateDoppler();
    }

    private void PRFSliderUpdate()
    {

        _dopplerVisualiser.Simulator.PulseRepetitionFrequency = prfSlider.CurrentValue;
        _dopplerVisualiser.UpdateDoppler();
    }

    private void AngleUpdate(int newAngle)
    {
        _dopplerVisualiser.Simulator.Angle = _raycastAngle.CurrentAngle;
        _dopplerVisualiser.UpdateDoppler();
    }

    private void OnDestroy()
    {
        bloodVelocitySlider.valueUpdate -= BloodVelocitySliderUpdate;
        prfSlider.valueUpdate -= PRFSliderUpdate;
        depthSlider.valueUpdate -= SamplingDepthSliderUpdate;
        _raycastAngle.valueUpdate -= AngleUpdate;
    }
}
