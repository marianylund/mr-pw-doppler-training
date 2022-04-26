using System;
using System.Collections;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace DopplerSim
{
    [RequireComponent(typeof(RawImage))]
    public class DopplerVisualiser : MonoBehaviour
    {
        public const float ConvertFromTrueToVisualised = 37f;
        public const float ConvertFromVisualisedToTrue = 1/ConvertFromTrueToVisualised;
        
        public delegate void OnDopplerVisualiser();
        public OnDopplerVisualiser dopplerUpdate;

        public bool ShowMaxValues = true;

        [SerializeField] private RectTransform labelTemplateY;
        [SerializeField] private RectTransform tickTemplateY;
        [SerializeField] private RectTransform tickTemplateX;
        [SerializeField] private RectTransform xAxis;
        [SerializeField] private RectTransform loadingLine;

        // "Max PRF: 22\tMax Velocity: ??"
        [SerializeField] private Text maxValues;

        public float MaxVelocity => _simulator.MaxVelocity * ConvertFromTrueToVisualised;
        public float MaxPRF => _simulator.MaxPRF;
        public float MaxArterialVelocity = 3.0f * ConvertFromTrueToVisualised;

        public float Angle
        {
            get => _simulator.Angle;
            set => _simulator.Angle = value;
        }

        public float ArterialVelocity
        {
            get => _simulator.ArterialVelocity * ConvertFromTrueToVisualised;
            set => _simulator.ArterialVelocity = value * ConvertFromVisualisedToTrue;
        }
        
        public float PulseRepetitionFrequency
        {
            get => _simulator.PulseRepetitionFrequency;
            set => _simulator.PulseRepetitionFrequency = value;
        }

        public float SamplingDepth
        {
            get => _simulator.SamplingDepth;
            set => _simulator.SamplingDepth = value;
        } 
        
        private RawImage _rawImage;
        private DopplerSimulator _simulator;

        private Coroutine _currentCoroutine;
        private Coroutine _secondCoroutine;

        private void Awake()
        {
            _rawImage = GetComponent<RawImage>();
            _simulator = new DopplerSimulator();
            _rawImage.texture = _simulator.CreatePlot();
            _rawImage.SetNativeSize();
            loadingLine.gameObject.SetActive(false);
            CreateAxis();
            UpdateMaxValues();
        }

        private void UpdateMaxValues()
        {
            if (ShowMaxValues)
            {
                string velocityColour = _simulator.IsVelocityOverMax ? "red" : "green";
                var roundedMaxVelocity = Mathf.Round(MaxVelocity * 10) / 10;
                maxValues.text = $"Max PRF: {Mathf.RoundToInt(MaxPRF)} kHz                      " +
                                 $"Max Velocity: <color={velocityColour}>{roundedMaxVelocity}</color> cm/s";
            }
            else
            {
                maxValues.text = "";
            }
        }

        private void CreateAxis()
        {
            const float gapY = 10f;
            const int velocityStepY = 30;
            const int timeStepX = velocityStepY;
            Transform parent = transform.parent;
            
            for (int tick = -4; tick < 6; tick++)
            {
                RectTransform tickY = Instantiate(tickTemplateY, parent);
                tickY.anchoredPosition = new Vector2(tickTemplateY.anchoredPosition.x, gapY * tick + xAxis.anchoredPosition.y);
                tickY.gameObject.SetActive(true);
                
                if(tick <= 0)
                    continue;
                RectTransform labelY = Instantiate(labelTemplateY, parent);
                labelY.anchoredPosition = new Vector2(labelTemplateY.anchoredPosition.x, gapY * tick + xAxis.anchoredPosition.y);
                labelY.gameObject.SetActive(true);
                labelY.GetComponent<Text>().text = (tick* 20f).ToString();
            }

            for (int tick = 1; tick < 7; tick++)
            {
                RectTransform tickX = Instantiate(tickTemplateX, parent);
                tickX.anchoredPosition = new Vector2( tickTemplateX.anchoredPosition.x - timeStepX * tick, tickTemplateX.anchoredPosition.y);
                tickX.gameObject.SetActive(true);
            }

        }

        public void UpdateDoppler()
        {
            UpdateMaxValues();
            if (_currentCoroutine == null)
            {
                _currentCoroutine = StartCoroutine(UpdateDopplerGraphRoutine(() => _currentCoroutine = null));
                dopplerUpdate?.Invoke();
            }
            else if (_secondCoroutine == null)
            {
                _secondCoroutine = StartCoroutine(UpdateDopplerGraphRoutine(() => _secondCoroutine = null));
                dopplerUpdate?.Invoke();
            }
        }

        private IEnumerator UpdateDopplerGraphRoutine(Action onFinish)
        {
            loadingLine.gameObject.SetActive(true);
            for (int t = _simulator.n_timepoints - 1; t >= 0; t--) // have to go in opposite direction to go from left to right
            {
                _simulator.UpdatePlot(t);
                loadingLine.anchoredPosition = new Vector2(_simulator.n_timepoints - t, 0);
                yield return new WaitForUpdate();
            }
            
            loadingLine.gameObject.SetActive(false);
            onFinish();
        }

        private void OnDisable()
        {
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
                _currentCoroutine = null;
            }
            if (_secondCoroutine != null)
            {
                StopCoroutine(_secondCoroutine);
                _secondCoroutine = null;
            }
        }
    }
}