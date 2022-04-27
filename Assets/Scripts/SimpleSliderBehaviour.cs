using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine.UI;

public class SimpleSliderBehaviour : MonoBehaviour
{
    public delegate void OnSliderEvent();
    public OnSliderEvent valueUpdate;
    [SerializeField]
    public Vector2 minMaxValue = Vector2.up;
    [SerializeField]
    private Text _currentValue;
    [SerializeField]
    private Text _minValue;
    [SerializeField]
    private Text _maxValue;
    private PinchSlider _pinchSlider;
    
    /// <summary>
    /// Interpolated value between min and max
    /// </summary>
    public float CurrentValue {  get; private set; }
    /// <summary>
    /// Non-interpolated current slider value
    /// </summary>
    public float CurrentRawValue { get; private set; }

    [SerializeField]
    private string floatAccuracy = "F0";
    // Start is called before the first frame update
    void Awake()
    {
        Debug.Assert(_currentValue != null, "CurrentValue textMesh is not set up in SimpleSliderBehaviour on " + gameObject.name);
        Debug.Assert(_minValue != null, "MinValue textMesh is not set up in SimpleSliderBehaviour on " + gameObject.name);
        Debug.Assert(_maxValue != null, "MaxValue textMesh is not set up in SimpleSliderBehaviour on " + gameObject.name);

        _pinchSlider = GetComponentInParent<PinchSlider>();
        if(_pinchSlider == null)
        {
            throw new MissingComponentException($"Parent of {gameObject.name} is missing PinchSlider component");
        }

        ChangeMinMaxValueText(minMaxValue.x, minMaxValue.y);
        //Debug.Log("Current value of " + name + " " + CurrentValue);
        _pinchSlider.OnValueUpdated.AddListener(OnSliderChange);
        _pinchSlider.OnInteractionEnded.AddListener((SliderEventData eventData) =>
        {
            valueUpdate?.Invoke();
        });
    }

    private void OnDestroy()
    {
        _pinchSlider.OnValueUpdated.RemoveListener(OnSliderChange);
    }

    public void OnSliderChange(SliderEventData data)
    {
        CurrentRawValue = data.NewValue;
        float newValue = Mathf.Lerp(
            minMaxValue.x, minMaxValue.y, data.NewValue);
        ChangeCurrentValueText(newValue);
    }

    public void ChangeCurrentValueText(float value)
    {
        CurrentValue = value;
        _currentValue.text = $"{value.ToString(floatAccuracy)}";
    }

    public void ChangeMinMaxValueText(float minValue, float maxValue)
    {
        _minValue.text = minValue.ToString(floatAccuracy);
        _maxValue.text = maxValue.ToString(floatAccuracy);
    }

    public void UpdateMaxValue(float maxValue)
    {
        if (Math.Abs(maxValue - minMaxValue.y) < 0.01f)
            return;
        minMaxValue.y = maxValue;
        _maxValue.text = maxValue.ToString(floatAccuracy);
    }


    /// <summary>
    /// expects a value between 0 and 1
    /// </summary>
    internal void SetNormalisedValue(float newValue)
    {
        _pinchSlider.SliderValue = newValue;
        CurrentRawValue = newValue;
    }
}