using DopplerSim;
using DopplerSim.Tools;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using Random = System.Random;


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

    private Random rand;
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
        
        rand = new Random();

        
        // _dopplerVisualiser.ArterialVelocity = bloodVelocitySlider.CurrentValue;
        // _dopplerVisualiser.SamplingDepth = depthSlider.CurrentValue;
        // _dopplerVisualiser.PulseRepetitionFrequency = prfSlider.CurrentValue;
        // _dopplerVisualiser.Angle = _raycastAngle.CurrentAngle;

        UpdateMaxValues(_dopplerVisualiser.MaxPRF, _dopplerVisualiser.MaxArterialVelocity);
        
        bloodVelocitySlider.valueUpdate += BloodVelocitySliderUpdate;
        prfSlider.valueUpdate += PRFSliderUpdate;
        depthSlider.valueUpdate += SamplingDepthSliderUpdate;
        _raycastAngle.valueUpdate += AngleUpdate;
    }
    
    public void UpdateMaxValues(float maxPRF, float maxVelocity)
    {
        bloodVelocitySlider.UpdateMaxValue(maxVelocity);
        prfSlider.UpdateMaxValue(maxPRF);
    }
    
    public void SetRandomBloodVelocityWithinRange()
    {
        var r = Mathf.Abs((float)rand.NextGaussian(mu:0.35, sigma:0.15));
        var lerpedRandomBloodVelocity = Mathf.Lerp(bloodVelocitySlider.minMaxValue.x, bloodVelocitySlider.minMaxValue.y, (float)r);
        Debug.Log($"r: {r}, mixMax: {bloodVelocitySlider.minMaxValue}, lerped: {lerpedRandomBloodVelocity}");
        bloodVelocitySlider.ChangeCurrentValueText(lerpedRandomBloodVelocity);
        _dopplerVisualiser.ArterialVelocity = lerpedRandomBloodVelocity;
        _dopplerVisualiser.UpdateDoppler();
    }
    
    public float GetBloodVelocity()
    {
        return _dopplerVisualiser.ArterialVelocity;
    }

    private void BloodVelocitySliderUpdate()
    {
        _dopplerVisualiser.ArterialVelocity = bloodVelocitySlider.CurrentValue;
        _dopplerVisualiser.UpdateDoppler();
    }

    private void SamplingDepthSliderUpdate()
    {
        // Depth is weird and needs to be 0.05 and 1.0 where around av_depth / 7.0D + 0.0125D + 0.05D
        // If av_depth is 3.0, so optimal depth is around 0.49107142857 is the optimum
        _dopplerVisualiser.SamplingDepth = Mathf.Clamp(prfSlider.CurrentRawValue, 0.05f, 1.0f);
        _dopplerVisualiser.UpdateDoppler();
    }

    private void PRFSliderUpdate()
    {
        _dopplerVisualiser.PulseRepetitionFrequency = prfSlider.CurrentValue;
        _dopplerVisualiser.UpdateDoppler();
    }

    private void AngleUpdate(int newAngle)
    {
        _dopplerVisualiser.Angle = _raycastAngle.CurrentAngle;
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
