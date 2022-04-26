using UnityEngine;

public class SlidersStateController : MonoBehaviour
{
    public bool ControlBloodVelocity = true;
    [SerializeField]
    private SimpleSliderBehaviour bloodVelocitySlider;
    [SerializeField]
    private SimpleSliderBehaviour inputBloodVelocitySlider;
    [SerializeField]
    private SimpleSliderBehaviour prfSlider;
    [SerializeField]
    private SimpleSliderBehaviour depthSlider;
    
    void Start()
    {
        ChangeVisibilityAll(false);
    }

    private void ChangeVisibilityAll(bool active)
    {
        bloodVelocitySlider.gameObject.SetActive(active);
        inputBloodVelocitySlider.gameObject.SetActive(active);
        prfSlider.gameObject.SetActive(active);
        depthSlider.gameObject.SetActive(active);
    }

    public void HideAll()
    {
        ChangeVisibilityAll(false);
    }

    public void SetMeasureState()
    {
        prfSlider.gameObject.SetActive(true);
        depthSlider.gameObject.SetActive(true);
        bloodVelocitySlider.gameObject.SetActive(ControlBloodVelocity);
        inputBloodVelocitySlider.gameObject.SetActive(false);
    }

    public void SetEstimateState()
    {
        prfSlider.gameObject.SetActive(true);
        depthSlider.gameObject.SetActive(true);
        bloodVelocitySlider.gameObject.SetActive(false);
        inputBloodVelocitySlider.gameObject.SetActive(true);
    }

    
}
